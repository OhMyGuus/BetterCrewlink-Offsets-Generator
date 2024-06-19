using DepotDownloader;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BCL_OffsetGenerator;

class SteamDownloadSource : IGameDownloadSource
{
    private SteamAccount _steamAccount => _config.SteamAccount;
    private readonly SteamDownloadSourceConfig _config;

    private static readonly DateTime ill2cppDate = new DateTime(2019, 11, 5, 0, 0, 0, DateTimeKind.Utc);

    public SteamDownloadSource()
    {
        _config = Config.Instance.SteamDownloadSourceConfig;
    }

    public async Task<List<MannifestInfo>> FetchManifests()
    {
        List<MannifestInfo> manifests = new List<MannifestInfo>();
        const string steamdbUrl = "http://127.0.0.1:8080/steamdb.html";

        //_httpClient = new HttpClient(handler);


        var httpHandler = new HttpClientHandler() { UseCookies = true, Proxy = GetProxy(), UseProxy = _config.Proxy.Enabled };
        using (HttpClient httpClient = new HttpClient(httpHandler))
        {


            //httpClient.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate, br");
            //httpHandler.CookieContainer.Add(new Uri(steamdbUrl),
            //    new Cookie("__Host-steamdb", _steamAccount.SteamDBCookie));
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
                    if (colum.FirstChild.Attributes.Any(o => o.Name == "datetime"))
                    {
                        var input = colum.FirstChild.GetAttributeValue("datetime", "");
                        manifest.Date = DateTime.Parse(input);
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


    private IWebProxy GetProxy()
    {
        return _config.Proxy.Enabled ? new WebProxy
        {
            Address = new Uri(_config.Proxy.Host),
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_config.Proxy.Username, _config.Proxy.Password)
        } : null;
    }
    public void Dispose()
    {
        ContentDownloader.ShutdownSteam3();
    }
}