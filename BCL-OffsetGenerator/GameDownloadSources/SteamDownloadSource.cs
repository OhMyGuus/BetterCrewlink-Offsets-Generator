using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DepotDownloader;
using HtmlAgilityPack;

namespace BCL_OffsetGenerator
{
    class SteamDownloadSource : IGameDownloadSource
    {
        private readonly SteamDownloadSourceConfig.SteamAccount _steamAccount;
        private static readonly DateTime ill2cppDate = new DateTime(2019, 11, 5, 0, 0, 0, DateTimeKind.Utc);

        public SteamDownloadSource()
        {
            _steamAccount = Config.Instance.SteamDownloadSourceConfig.account;
        }

        public async Task<List<MannifestInfo>> FetchManifests()
        {

            return new List<MannifestInfo>()
            {
                new MannifestInfo() { ManifestId = 6859044017310390774, x64 = false, Date = DateTime.Parse("2022-07-13T18:59:02.000Z") },
                new MannifestInfo() { ManifestId = 7098949171778272067, x64 = false, Date = DateTime.Parse("2022-06-21T16:59:56.000Z") },
                new MannifestInfo() { ManifestId = 1994315496259700450, x64 = false, Date = DateTime.Parse("2022-03-31T16:59:41.000Z") },
                new MannifestInfo() { ManifestId = 3765974117789159936, x64 = false, Date = DateTime.Parse("2022-02-25T01:52:49.000Z") },
                new MannifestInfo() { ManifestId = 4946739663065807500, x64 = false, Date = DateTime.Parse("2022-02-24T21:09:12.000Z") },
                new MannifestInfo() { ManifestId = 623947946057191577, x64 = false, Date = DateTime.Parse("2022-02-16T22:03:10.000Z") },
                new MannifestInfo() { ManifestId = 407539178195941172, x64 = false, Date = DateTime.Parse("2021-12-16T20:34:21.000Z") },
                new MannifestInfo() { ManifestId = 781144051117197771, x64 = false, Date = DateTime.Parse("2021-12-14T18:59:50.000Z") },
                new MannifestInfo() { ManifestId = 7660376491134459569, x64 = false, Date = DateTime.Parse("2021-11-11T21:36:23.000Z") },
                new MannifestInfo() { ManifestId = 6548228893653594809, x64 = false, Date = DateTime.Parse("2021-11-10T20:26:38.000Z") },
                new MannifestInfo() { ManifestId = 3711406830853445086, x64 = false, Date = DateTime.Parse("2021-11-09T19:00:16.000Z") },
                new MannifestInfo() { ManifestId = 3510344350358296660, x64 = false, Date = DateTime.Parse("2021-07-07T16:01:46.000Z") },
                new MannifestInfo() { ManifestId = 7234669263252285340, x64 = false, Date = DateTime.Parse("2021-06-15T19:00:42.000Z") },
                new MannifestInfo() { ManifestId = 171532832512637061, x64 = false, Date = DateTime.Parse("2021-05-27T22:29:23.000Z") },
                new MannifestInfo() { ManifestId = 3505836702083416897, x64 = false, Date = DateTime.Parse("2021-05-26T21:37:06.000Z") },
                new MannifestInfo() { ManifestId = 6640729220842422804, x64 = false, Date = DateTime.Parse("2021-05-10T20:01:37.000Z") },
                new MannifestInfo() { ManifestId = 1328515537648622656, x64 = false, Date = DateTime.Parse("2021-04-14T21:26:09.000Z") },
                new MannifestInfo() { ManifestId = 2725166875026424465, x64 = false, Date = DateTime.Parse("2021-04-12T21:03:38.000Z") },
                new MannifestInfo() { ManifestId = 3941730972865408291, x64 = false, Date = DateTime.Parse("2021-04-01T03:53:05.000Z") },
                new MannifestInfo() { ManifestId = 2517455626042488615, x64 = false, Date = DateTime.Parse("2021-03-31T21:59:24.000Z") },
                new MannifestInfo() { ManifestId = 9177722260012976528, x64 = false, Date = DateTime.Parse("2021-03-31T18:01:52.000Z") },
                new MannifestInfo() { ManifestId = 5200448423569257054, x64 = false, Date = DateTime.Parse("2021-03-05T21:49:35.000Z") },
                new MannifestInfo() { ManifestId = 6261260287144428102, x64 = false, Date = DateTime.Parse("2021-03-05T21:41:34.000Z") },
                new MannifestInfo() { ManifestId = 7741948098992377220, x64 = false, Date = DateTime.Parse("2021-03-05T20:22:17.000Z") },
                new MannifestInfo() { ManifestId = 2052794034769266028, x64 = false, Date = DateTime.Parse("2021-03-05T19:15:54.000Z") },
                new MannifestInfo() { ManifestId = 3306639722673334636, x64 = false, Date = DateTime.Parse("2020-12-10T22:43:35.000Z") },
                new MannifestInfo() { ManifestId = 7819696192136794230, x64 = false, Date = DateTime.Parse("2020-12-05T20:59:53.000Z") },
                new MannifestInfo() { ManifestId = 8030402202141329675, x64 = false, Date = DateTime.Parse("2020-12-04T18:38:16.000Z") },
                new MannifestInfo() { ManifestId = 2101726827626605419, x64 = false, Date = DateTime.Parse("2020-11-24T18:32:30.000Z") },
                new MannifestInfo() { ManifestId = 982761632128031721, x64 = false, Date = DateTime.Parse("2020-11-23T23:54:53.000Z") },
                new MannifestInfo() { ManifestId = 7477775776970303363, x64 = false, Date = DateTime.Parse("2020-11-03T01:08:04.000Z") },
                new MannifestInfo() { ManifestId = 8969023746453332691, x64 = false, Date = DateTime.Parse("2020-09-24T21:02:17.000Z") },
                new MannifestInfo() { ManifestId = 3596575937380717449, x64 = false, Date = DateTime.Parse("2020-09-14T15:41:25.000Z") },
                new MannifestInfo() { ManifestId = 5957631781288795948, x64 = false, Date = DateTime.Parse("2020-09-02T03:04:02.000Z") },
                new MannifestInfo() { ManifestId = 3948341867684505905, x64 = false, Date = DateTime.Parse("2020-09-02T03:03:47.000Z") },
                new MannifestInfo() { ManifestId = 8820802508818016315, x64 = false, Date = DateTime.Parse("2020-09-01T01:31:32.000Z") },
                new MannifestInfo() { ManifestId = 8698757747328064493, x64 = false, Date = DateTime.Parse("2020-08-18T23:14:41.000Z") },
                new MannifestInfo() { ManifestId = 7185019524463673033, x64 = false, Date = DateTime.Parse("2020-08-12T18:37:12.000Z") },
                new MannifestInfo() { ManifestId = 790870617990906244, x64 = false, Date = DateTime.Parse("2020-06-11T20:03:17.000Z") },
                new MannifestInfo() { ManifestId = 6320154229190358631, x64 = false, Date = DateTime.Parse("2020-06-11T17:50:08.000Z") },
                new MannifestInfo() { ManifestId = 8810314642632014619, x64 = false, Date = DateTime.Parse("2020-05-10T19:11:59.000Z") },
                new MannifestInfo() { ManifestId = 475238497445568226, x64 = false, Date = DateTime.Parse("2020-04-04T18:42:29.000Z") },
                new MannifestInfo() { ManifestId = 3994640171417669001, x64 = false, Date = DateTime.Parse("2020-03-30T19:55:43.000Z") },
                new MannifestInfo() { ManifestId = 2626681146878071561, x64 = false, Date = DateTime.Parse("2020-02-18T05:29:52.000Z") },
                new MannifestInfo() { ManifestId = 7941316833749143242, x64 = false, Date = DateTime.Parse("2020-02-10T22:38:59.000Z") },
                new MannifestInfo() { ManifestId = 878284256271052527, x64 = false, Date = DateTime.Parse("2020-01-14T22:03:45.000Z") },
                new MannifestInfo() { ManifestId = 7101495731544490901, x64 = false, Date = DateTime.Parse("2019-11-12T17:54:28.000Z") },
                new MannifestInfo() { ManifestId = 9176470343280174159, x64 = false, Date = DateTime.Parse("2019-11-05T22:23:14.000Z") },
                new MannifestInfo() { ManifestId = 8288448455957108838, x64 = false, Date = DateTime.Parse("2019-11-05T21:24:52.000Z") },
            };

            List<MannifestInfo> manifests = new List<MannifestInfo>();
            const string steamdbUrl = "https://steamdb.info/depot/945361/manifests/";
            var httpHandler = new HttpClientHandler() { UseCookies = true };
            using (HttpClient httpClient = new HttpClient(httpHandler))
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                httpHandler.CookieContainer.Add(new Uri(steamdbUrl),
                    new Cookie("__Host-steamdb", _steamAccount.SteamDBCookie));
                var steamdbHtml = await httpClient.GetStringAsync(steamdbUrl);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(steamdbHtml);
                var notes = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='manifests']").SelectNodes(".//tr");
                foreach (var note in notes)
                {
                    var colums = note.SelectNodes(".//td");
                    var codes = note.SelectNodes(".//code");

                    if (colums == null || codes != null)
                        continue;

                    var manifest = new MannifestInfo() { x64 = false };
                    foreach (var colum in colums)
                    {
                        if (colum.HasClass("timeago"))
                        {
                            manifest.Date = DateTime.Parse(colum.GetDataAttribute("time")?.Value ?? "");
                        }

                        if (colum.HasClass("tabular-nums"))
                        {
                            manifest.ManifestId = Convert.ToUInt64(colum.SelectSingleNode(".//a")?.InnerText ?? "0");
                        }
                    }

                    if (manifest.Date != null && manifest.ManifestId > 0 && manifest.Date > ill2cppDate)
                    {
                        manifests.Add(manifest);
                    }
                }
            }

            return manifests.OrderByDescending(o => o.Date).ToList();
        }

        public async Task DownloadManifests(List<MannifestInfo> manifests, bool skipDownload = false)
        {
            InitializeContentDownloader();

            //  await ContentDownloader.DownloadAppAsync(945360, manifests.Select(o => ((uint)945361, o.ManifestId)).ToList(), ContentDownloader.DEFAULT_BRANCH, null, null, null, false, true);

            foreach (var manifest in manifests.ToArray())
            {
                if (!await DownloadManifest(manifest, skipDownload))
                {
                    manifests.Remove(manifest);
                }
            }
        }

        public void InitializeContentDownloader()
        {
            AccountSettingsStore.LoadFromFile("account.config");
            ContentDownloader.Config.UsingFileList = true;
            ContentDownloader.Config.MaxDownloads = 8;
            ContentDownloader.Config.MaxServers = 20;
            ContentDownloader.Config.InstallDirectory = Constants.AMONGUSFILES_PATH;
            ContentDownloader.Config.FilesToDownloadRegex = new List<Regex>();
            ContentDownloader.Config.FilesToDownload = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "GameAssembly.dll",
                "Among Us_Data/globalgamemanagers",
                "Among Us_Data/il2cpp_data/Metadata/global-metadata.dat",
                "MonoBleedingEdge/EmbedRuntime/MonoPosixHelper.dll"
            };
            DepotDownloader.Program.InitializeSteam(_steamAccount.Username, _steamAccount.Password);
        }

        private bool downloadedAllFiles(string folder) => (Directory.Exists(folder)
                                                           && File.Exists($"{folder}/GameAssembly.dll")
                                                           && File.Exists($"{folder}/Among Us_Data/globalgamemanagers")
                                                           && File.Exists(
                                                               $"{folder}/Among Us_Data/il2cpp_data/Metadata/global-metadata.dat"));

        public async Task<bool> DownloadManifest(MannifestInfo manifest, bool skipDownload = false)
        {
            var folder = $"{Constants.AMONGUSFILES_PATH}/{manifest.ManifestId}";

            if (downloadedAllFiles(folder))
                return true;
            if (skipDownload || File.Exists($"{folder}/Among Us_Data/il2cpp_data/Metadata/global-metadata.dat"))
                return false;

            DepotConfigStore.Instance = null;
            ContentDownloader.Config.InstallDirectory = folder;
            await ContentDownloader.DownloadAppAsync(945360, new List<(uint, ulong)> { (945361, manifest.ManifestId) },
                ContentDownloader.DEFAULT_BRANCH, null, null, null, false, true);

            return downloadedAllFiles(folder);
        }

        public static bool Enabled => Config.Instance.SteamDownloadSourceConfig.Enabled;

        public void Dispose()
        {
            ContentDownloader.ShutdownSteam3();
        }
    }
}