using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BCL_OffsetGenerator.GameDownloadSources;
using CommandLine;
using Newtonsoft.Json;
using static BCL_OffsetGenerator.LookupJson;

namespace BCL_OffsetGenerator
{
    class Program
    {
        //todo: move this to a class 
        private static List<MannifestInfo> _manifests = new List<MannifestInfo>();

        //todo: allow arguments and move certain things to other classes.
        static async Task Main(string[] args)
        {
        
            Console.WriteLine("Starting BetterCrewlink Offset generator");
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            ReadManifests();
            SaveManifest();
            Config.Instance.Load("config.json");

            if (!Config.Instance.LoadArgs(args))
                return;

            foreach (var type in IGameDownloadSource.DownloadSources)
            {
                if (!(type?.GetProperty("Enabled")?.GetValue(null) as bool? ?? false) ||
                    Activator.CreateInstance(type) is not IGameDownloadSource downloadSource)
                    continue;

                using (downloadSource)
                {
                    Console.WriteLine("Starting source: {0}", type);

                    var sourceManifests = await downloadSource.FetchManifests();
                    if(sourceManifests == null)
                        continue;
                    //foreach (var manifest in sourceManifests)
                    //{
                    //    // new MannifestInfo(){ ManifestId = manifest.ManifestId, x64 = manifest.x64, Date = manifest.Date, Version = manifest.Version, Obfucated = manifest.Obfucated, BroadcastVersion = manifest.BroadcastVersion, InnerNetClient = manifest.InnerNetClient};
                    //    Console.WriteLine($" new MannifestInfo(){{ ManifestId = {manifest.ManifestId}, x64 = {manifest.x64.ToString().ToLower()}, Date = DateTime.Parse(\"{manifest.Date.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'")}\") }},");
                    //}
                    await downloadSource.DownloadManifests(sourceManifests, false);
                    bool newManifests = AddNewManifests(sourceManifests, out _);
                }
            }

            var dumper = new GameIll2CppDumper();
            dumper.DumpGameFiles(_manifests);
            var gameInfoExtractor = new GameInfoExtractor();
            gameInfoExtractor.FillManifests(_manifests);
            SaveManifest();

            await GenerateOffsets();
            SaveManifest();
            Console.WriteLine("Done");
        }

        //todo: move
        private static bool AddNewManifests(List<MannifestInfo> manifests, out List<MannifestInfo> addedManifests)
        {
            var newManifests = manifests.Where(o => _manifests.All(b => b.ManifestId != o.ManifestId)).ToList();
            _manifests.AddRange(newManifests);
            _manifests = _manifests.OrderByDescending(o => o.Date).ToList();
            addedManifests = newManifests;

            return newManifests.Count > 0;
        }

        //todo: move
        private static void ReadManifests()
        {
            if (File.Exists($"{Constants.WORKINGDIRECTORY_PATH}/manifest.json"))
            {
                _manifests =
                    (JsonConvert.DeserializeObject<List<MannifestInfo>>(File.ReadAllText($"{Constants.WORKINGDIRECTORY_PATH}/manifest.json"))
                         ?.OrderByDescending(o => o.Date)?.ToList() ??
                     new List<MannifestInfo>());
            }
        }

        //todo: move
        private static void SaveManifest()
        {
            new FileInfo($"{Constants.WORKINGDIRECTORY_PATH}/manifest.json")?.Directory?.Create();
            new FileInfo($"{Constants.OUTPUT_PATH}/manifest.json")?.Directory?.Create();
            File.WriteAllText($"{Constants.WORKINGDIRECTORY_PATH}/manifest.json", JsonConvert.SerializeObject(_manifests, Formatting.Indented));
            File.WriteAllText($"{Constants.OUTPUT_PATH}/manifest.json", JsonConvert.SerializeObject(_manifests, Formatting.Indented));
        }


        //temp still needs to be rewritten
        private static async Task GenerateOffsets()
        {
            var offsets = new Dictionary<List<MannifestInfo>, string>();
            int index = 0;
            var filteredManifests = _manifests.Where(o =>
                !o.Obfucated && o.InnerNetClient != 0 && !string.IsNullOrWhiteSpace(o.Version)).OrderByDescending(o => o.BroadcastVersion);
            foreach (var manifest in filteredManifests) //.Where(o => o.Version == "V2021.6.30s"))
            {
                Console.WriteLine("Getting offsets for {0} -> {1}:{2}", manifest.Version, index++, _manifests.Count);
                var offsetGenerator = new GameOffsetsGenerator(manifest);
                var result = offsetGenerator.Generate();
                var json = JsonConvert.SerializeObject(result, new JsonSerializerSettings() { Formatting = Formatting.Indented });
                var existingManifest = offsets.FirstOrDefault(o => o.Value == json);
                existingManifest.Key?.Add(manifest);

                if (existingManifest.Key == null)
                    offsets.Add(new List<MannifestInfo>() { manifest }, json);
                // File.WriteAllText("result.json", JsonSerializer.Serialize(result, options: new JsonSerializerOptions { WriteIndented = true }));
            }

            Directory.CreateDirectory($"{Constants.OUTPUT_PATH}/offsets");

            foreach (var item in offsets)
            {
                var manifest = item.Key.LastOrDefault();
                var path = $"{Constants.OUTPUT_PATH}/offsets/{(manifest.x64 ? "x64" : "x86")}/{manifest.VersionWithoutGame}/";
                Directory.CreateDirectory(path);
                File.WriteAllText(path + "manifests.json",
                 JsonConvert.SerializeObject(item.Key, Formatting.Indented));
                File.WriteAllText(path + "offsets.json", item.Value);
            }

            var lookup = await LookupJson.GetFrombase();


            foreach (var offsetGen in offsets)
            {
                foreach (var manifest in offsetGen.Key)
                {
                    lookup.Versions[manifest.BroadcastVersion.Value.ToString()] = new LookupVersions()
                    {
                        version = manifest.VersionWithoutGame, file = $"{offsetGen.Key.LastOrDefault().VersionWithoutGame}/offsets.json",
                        offsetsVersion = Constants.CURRENT_OFFSET_VERSION
                    };
                }
            }

            if (lookup.Versions.Count > 1)
                lookup.Versions["default"] = lookup.Versions.Skip(1).FirstOrDefault().Value;

            File.WriteAllText($"{Constants.OUTPUT_PATH}/lookup.json",
                JsonConvert.SerializeObject(lookup, Formatting.Indented));
        }
    }
}