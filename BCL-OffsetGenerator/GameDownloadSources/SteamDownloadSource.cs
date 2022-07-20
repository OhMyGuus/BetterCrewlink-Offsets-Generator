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
    class SteamDownloadSource : IDisposable, IGameDownloadSource
    {
        private readonly SteamAccount _steamAccount;
        private static readonly DateTime ill2cppDate = new DateTime(2019,11,6);
        public SteamDownloadSource(SteamAccount account)
        {
            _steamAccount = account;
        }

        public async Task<List<MannifestInfo>> FetchManifests()
        {
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

        public void Dispose()
        {
            ContentDownloader.ShutdownSteam3();
        }
    }

    internal class SteamAccount
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string SteamDBCookie { get; set; }
    }
}