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
    class SteamGameSource : IDisposable
    {
        private readonly SteamAccount _steamAccount;

        public SteamGameSource(SteamAccount account)
        {
            _steamAccount = account;
        }

        public async Task<List<MannifestInfo>> FetchManifests()
        {
            List<MannifestInfo> manifests = new List<MannifestInfo>();
            const string STEAMDB_URL = "https://steamdb.info/depot/945361/manifests/";
            var httpHandler = new HttpClientHandler() { UseCookies = true };
            using (HttpClient httpClient = new HttpClient(httpHandler))
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                httpHandler.CookieContainer.Add(new Uri(STEAMDB_URL),
                    new Cookie("__Host-steamdb", _steamAccount.SteamDBCookie));
                var steamdbHtml = await httpClient.GetStringAsync(STEAMDB_URL);
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

                    if (manifest.Date != null && manifest.ManifestId > 0)
                    {
                        manifests.Add(manifest);
                    }
                }
            }

            return manifests;
        }

        public async Task DownloadManifests(List<MannifestInfo> manifests)
        {
            InitializeContentDownloader();

            //  await ContentDownloader.DownloadAppAsync(945360, manifests.Select(o => ((uint)945361, o.ManifestId)).ToList(), ContentDownloader.DEFAULT_BRANCH, null, null, null, false, true);

            foreach (var manifest in manifests)
            {
                await DownloadManifest(manifest);
            }
        }

        public void InitializeContentDownloader()
        {
            AccountSettingsStore.LoadFromFile("account.config");
            ContentDownloader.Config.UsingFileList = true;
            ContentDownloader.Config.MaxDownloads = 8;
            ContentDownloader.Config.MaxServers = 20;
            ContentDownloader.Config.InstallDirectory = "AmongUsFiles";
            ContentDownloader.Config.FilesToDownloadRegex = new List<Regex>();
            ContentDownloader.Config.FilesToDownload = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "GameAssembly.dll",
                "Among Us_Data/globalgamemanagers",
                "Among Us_Data/il2cpp_data/Metadata/global-metadata.dat"
            };
            DepotDownloader.Program.InitializeSteam(_steamAccount.Username, _steamAccount.Password);
        }


        public async Task DownloadManifest(MannifestInfo manifest)
        {
            //  DepotDownloader
            // await ContentDownloader.DownloadAppAsync(945360, 945361, manifest.ManifestId);
            //C:\Users\GuusM\source\repos\BCL-OffsetGenerator\BCL-OffsetGenerator\DepotDownloader
            // var a = Assembly.Load(bytes);
            var folder = $"AmongUsFiles/{manifest.ManifestId}";

            if (Directory.Exists(folder)
                && File.Exists($"{folder}/GameAssembly.dll")
                && File.Exists($"{folder}/Among Us_Data/globalgamemanagers")
                && File.Exists($"{folder}/Among Us_Data/il2cpp_data/Metadata/global-metadata.dat")
               )
                return;

            DepotConfigStore.Instance = null;
            ContentDownloader.Config.InstallDirectory = folder;
            await ContentDownloader.DownloadAppAsync(945360, new List<(uint, ulong)> { (945361, manifest.ManifestId) },
                ContentDownloader.DEFAULT_BRANCH, null, null, null, false, true);

            // var process = new Process
            // {
            //     StartInfo = new ProcessStartInfo
            //     {
            //         FileName = @"DepotDownloader\DepotDownloader.exe",
            //         Arguments = String.Join(" ", new string[]
            //         {
            //             "-app 945360",
            //             "-depot 945361",
            //             $"-username {steamAccount.Username}",
            //             $"-password {steamAccount.Password}",
            //             $"-manifest {manifest.ManifestId}",
            //             "-filelist DepotDownloader/files.txt",
            //             $"-dir AmongUsFiles/{manifest.ManifestId}"
            //         }),
            //         UseShellExecute = false,
            //         RedirectStandardOutput = false,
            //         CreateNoWindow = false,
            //     }
            // };
            // process.Start();
            // //while (!process.StandardOutput.EndOfStream)
            // //{
            // //    string line = process.StandardOutput.ReadLine();
            // //    // do something with line
            // //}
            // process.WaitForExit();
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