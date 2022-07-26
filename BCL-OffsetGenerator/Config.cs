using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
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
        var json = JsonConvert.SerializeObject(Instance, Formatting.Indented);
        File.WriteAllText(filename, json);
    }


    public bool LoadArgs(string[] args)
    {
        var result = Parser.Default.ParseArguments<CommandLineConfig>(args);
        result.WithParsed((Action<CommandLineConfig>)(commandConfig =>
        {
            Instance.LocalGameSourceConfig.Enabled = commandConfig.LocalEnabled ?? Instance.LocalGameSourceConfig.Enabled;
            Instance.LocalGameSourceConfig.Path = commandConfig.LocalPath ?? Instance.LocalGameSourceConfig.Path;
            Instance.SteamDownloadSourceConfig.SteamAccount.Username =
                commandConfig.SteamUsername ?? Instance.SteamDownloadSourceConfig.SteamAccount.Username;
            Instance.SteamDownloadSourceConfig.SteamAccount.Password =
                commandConfig.SteamPassword ?? Instance.SteamDownloadSourceConfig.SteamAccount.Password;
            Instance.SteamDownloadSourceConfig.SteamAccount.SteamDBCookie =
                commandConfig.SteamDBCookie ?? Instance.SteamDownloadSourceConfig.SteamAccount.SteamDBCookie;
            Instance.SteamDownloadSourceConfig.Enabled = commandConfig.SteamEnabled ?? Instance.SteamDownloadSourceConfig.Enabled;
            Instance.SteamDownloadSourceConfig.Proxy.Host = commandConfig.ProxyHost ?? Instance.SteamDownloadSourceConfig.Proxy.Host;
            Instance.SteamDownloadSourceConfig.Proxy.Username = commandConfig.ProxyUsername ?? Instance.SteamDownloadSourceConfig.Proxy.Username;
            Instance.SteamDownloadSourceConfig.Proxy.Password = commandConfig.ProxyPassword ?? Instance.SteamDownloadSourceConfig.Proxy.Password;

        }));
        Save("config.json");

        return !result.Errors.Any();
    }
}

internal class SteamDownloadSourceConfig
{
    public bool Enabled { get; set; }

    public SteamAccount SteamAccount { get; set; } = new SteamAccount();
    public Proxy Proxy { get; set; } = new Proxy();

}

internal class Proxy
{
    public bool Enabled => !string.IsNullOrEmpty(Host);
    public string? Host { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }

}
internal class SteamAccount
{
    public string Username { get; set; } = "";

    public string Password { get; set; } = "";

    public string SteamDBCookie { get; set; } = "";
}

public class LocalGameSourceConfig
{
    public bool Enabled { get; set; }
    public string Path { get; set; } = "";
}


internal class CommandLineConfig
{
    [Option("steamenabled", Required = false, HelpText = "Steam enabled")]
    public bool? SteamEnabled { get; set; }
    [Option("steamusername", Required = false, HelpText = "Steam username")]
    public string? SteamUsername { get; set; }

    [Option("steampassword", Required = false, HelpText = "Steam password")]
    public string? SteamPassword { get; set; }

    [Option("steamdbcookie", Required = false, HelpText = "SteamDBCookie")]
    public string? SteamDBCookie { get; set; }

    [Option("localenabled", Required = false, HelpText = "Local path")]
    public bool? LocalEnabled { get; set; }

    [Option("localpath", Required = false, HelpText = "Local path")]
    public string? LocalPath { get; set; }

    [Option("proxy_host", Required = false, HelpText = "Proxyhost like http://example.com:3128")]
    public string? ProxyHost { get; set; }

    [Option("proxy_username", Required = false, HelpText = "Proxy username")]
    public string? ProxyUsername { get; set; }
    [Option("proxy_password", Required = false, HelpText = "Proxy password")]
    public string? ProxyPassword { get; set; }

}