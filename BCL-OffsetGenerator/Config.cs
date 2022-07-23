using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using static BCL_OffsetGenerator.GameDownloadSources.LocalDownloadSource;

namespace BCL_OffsetGenerator;
internal class Config
{
    private static Config _instance;

    public static Config Instance
    {
        get { return _instance ??= new Config(); }
    }

    public LocalGameSourceConfig LocalGameSourceConfig { get; set; } = new LocalGameSourceConfig();
    public SteamDownloadSourceConfig SteamDownloadSourceConfig { get; set; } = new SteamDownloadSourceConfig();

    public void Load(string filename)
    {
        if (File.Exists(filename))
        {
            var json = File.ReadAllText(filename);
            if ((string.IsNullOrEmpty(json) ? null : JsonConvert.DeserializeObject<Config>(json)) is { } config)
            {
                _instance = config;
                return;
            }
        }

        Save(filename);
    }

    public void Save(string filename)
    {
        var json = JsonConvert.SerializeObject(_instance, Formatting.Indented);
        File.WriteAllText(filename, json);
    }
}

internal class SteamDownloadSourceConfig
{
    public bool Enabled { get; set; }
    public SteamAccount account { get; set; } = new SteamAccount();

    internal class SteamAccount
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string SteamDBCookie { get; set; } = "";
    }
}

public class LocalGameSourceConfig
{
    public bool Enabled { get; set; }
    public string Path { get; set; } = "";
}