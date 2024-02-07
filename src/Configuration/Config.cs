using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Vintagestory.API.Config;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace PlayerList.Configuration;

public class Config {
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [YamlMember(Order = 0, Description = "Ping thresholds for good, poor, and bad ping times (in milliseconds)")]
    public int[]? Thresholds { get; set; } = { 65, 125, 500 };

    private static Config? _config;
    private static FileWatcher? _watcher;

    public static string ConfigFile => Path.Combine(GamePaths.ModConfig, $"{PlayerListMod.Instance.Mod.Info.ModID}.yml");

    public static int[]? PingThresholds => (_config ??= Reload()).Thresholds;
    public static int[]? ServerPingThresholds { get; set; }

    public static Config Reload() {
        PlayerListMod.Instance.Mod.Logger.Event($"Loading config from {ConfigFile}");

        _config = Write(Read());

        _watcher ??= new FileWatcher();

        return _config;
    }

    private static Config Read() {
        try {
            return new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .WithNamingConvention(NullNamingConvention.Instance)
                .Build().Deserialize<Config>(File.ReadAllText(ConfigFile));
        } catch (Exception) {
            return new Config();
        }
    }

    private static Config Write(Config config) {
        GamePaths.EnsurePathExists(GamePaths.ModConfig);
        File.WriteAllText(ConfigFile,
            new SerializerBuilder()
                .WithQuotingNecessaryStrings()
                .WithNamingConvention(NullNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build().Serialize(config)
            , Encoding.UTF8);
        return config;
    }

    public static void Dispose() {
        _watcher?.Dispose();
        _watcher = null;
    }
}
