using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using playerlist.configuration;
using playerlist.gui;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace playerlist;

public class PlayerList : ModSystem {
    public ICoreAPI Api { get; private set; } = null!;

    public ILogger Logger => Mod.Logger;
    public string ModId => Mod.Info.ModID;

    private Config? _config;
    private FileWatcher? _fileWatcher;
    private PlayerListHud? _hud;
    private IServerNetworkChannel? _channel;
    private int[]? _serverThresholds;

    public int[] Thresholds => _serverThresholds ?? _config?.Thresholds ?? new[] { 65, 125, 500 };

    public override void StartPre(ICoreAPI api) {
        Api = api;
        ReloadConfig();
    }

    public override void StartClientSide(ICoreClientAPI capi) {
        _hud = new PlayerListHud(this);

        capi.Network.RegisterChannel(Mod.Info.ModID)
            .RegisterMessageType<Config>()
            .SetMessageHandler<Config>(packet => {
                Mod.Logger.Event($"Received ping thresholds of {string.Join(",", packet.Thresholds!)} from server");
                _serverThresholds = packet.Thresholds;
            });
    }

    public override void StartServerSide(ICoreServerAPI sapi) {
        _channel = sapi.Network.RegisterChannel(Mod.Info.ModID)
            .RegisterMessageType<Config>()
            .SetMessageHandler<Config>((_, _) => { });

        sapi.Event.PlayerJoin += OnPlayerJoin;
    }

    private void OnPlayerJoin(IServerPlayer player) {
        _channel?.SendPacket(_config, player);
    }

    public void ReloadConfig() {
        _config = Api.LoadModConfig<Config>($"{ModId}.json") ?? new Config();

        (_fileWatcher ??= new FileWatcher(this)).Queued = true;

        string json = JsonConvert.SerializeObject(_config, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        FileInfo fileInfo = new(Path.Combine(GamePaths.ModConfig, $"{ModId}.json"));
        GamePaths.EnsurePathExists(fileInfo.Directory!.FullName);
        File.WriteAllText(fileInfo.FullName, json);

        Api.Event.RegisterCallback(_ => _fileWatcher.Queued = false, 100);
    }

    public override void Dispose() {
        _hud?.Dispose();
        _hud = null;

        _fileWatcher?.Dispose();
        _fileWatcher = null;

        if (Api is ICoreServerAPI sapi) {
            sapi.Event.PlayerJoin -= OnPlayerJoin;
            _channel = null;
        }
    }
}
