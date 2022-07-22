using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace BCL_OffsetGenerator.GameDownloadSources;
public class LocalDownloadSource : IGameDownloadSource
{
    private readonly LocalGameSourceConfig _config;

    public LocalDownloadSource(LocalGameSourceConfig config)
    {
        _config = config;
    }

    public async Task<List<MannifestInfo>> FetchManifests()
    {
        return !File.Exists(Path.Combine(_config.Path, "manifest.json"))
            ? null
            : JsonConvert.DeserializeObject<List<MannifestInfo>>(
                await File.ReadAllTextAsync(Path.Combine(_config.Path, "manifest.json")));
    }

    public async Task DownloadManifests(List<MannifestInfo> manifests, bool skipDownload = false)
    {
        if (manifests == null)
            return;

        foreach (var manifest in manifests)
        {
            if (Directory.Exists(Path.Combine(_config.Path, "AmongUsFiles", manifest.ManifestId.ToString())))
            {
                var folder = $"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}";
                Directory.CreateDirectory(folder);
                foreach (var file in Constants.REQUIRED_GAMEFILES)
                {
                    var originalPath = Path.Combine(_config.Path, "AmongUsFiles", manifest.ManifestId.ToString(), file);
                    var newPath = Path.Combine(folder, file);

                    if (!File.Exists(originalPath) || File.Exists(newPath))
                        continue;

                    Directory.CreateDirectory(Path.GetDirectoryName(newPath));
                    File.Copy(originalPath, newPath);
                }
            }
        }
    }


    public class LocalGameSourceConfig
    {
        public bool Enabled { get; set; }
        public string Path { get; set; }
    }

    public void Dispose()
    {
    }
}