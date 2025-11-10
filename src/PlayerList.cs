using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using playerlist.configuration;
using playerlist.gui;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using Vintagestory.Server;

namespace playerlist;

[UsedImplicitly]
public class PlayerList : ModSystem {
    public ICoreAPI Api { get; private set; } = null!;

    public ILogger Logger => Mod.Logger;
    public string ModId => Mod.Info.ModID;

    public Config Config => _serverConfig ?? _config!;
    private Config? _config;
    private Config? _serverConfig;

    private FileWatcher? _fileWatcher;
    private PlayerListHud? _hud;
    private IServerNetworkChannel? _channel;
    private PingIcon? _pingIcon;

    public override void StartPre(ICoreAPI api) {
        Api = api;
        ReloadConfig();
    }

    public override void StartClientSide(ICoreClientAPI capi) {
        _hud = new PlayerListHud(this);
        _pingIcon = new PingIcon(capi);

        capi.Network.RegisterChannel(Mod.Info.ModID)
            .RegisterMessageType<Config>()
            .SetMessageHandler<Config>(serverConfig => {
                Mod.Logger.Event("Received config from the server");
                _serverConfig = serverConfig;
            });
    }

    public override void StartServerSide(ICoreServerAPI sapi) {
        _channel = sapi.Network.RegisterChannel(Mod.Info.ModID)
            .RegisterMessageType<Config>()
            .SetMessageHandler<Config>((_, _) => {
            });

        sapi.Event.PlayerJoin += OnPlayerJoin;
    }

    private void OnPlayerJoin(IServerPlayer player) {
        ServerMain server = (ServerMain)player.Entity.World;
        _config!.MaxPlayers = server.Config.GetMaxClients(server);
        _channel?.SendPacket(_config, player);
    }

    public void ReloadConfig() {
        GamePaths.EnsurePathExists(GamePaths.ModConfig);

        _config = Api.LoadModConfig<Config>($"{ModId}.json") ?? new Config();

        string json = JsonConvert.SerializeObject(_config, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = Api is ICoreServerAPI ? NullValueHandling.Include : NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        (_fileWatcher ??= new FileWatcher(this)).Queued = true;

        File.WriteAllText(Path.Combine(GamePaths.ModConfig, $"{ModId}.json"), json);

        Api.Event.RegisterCallback(_ => _fileWatcher.Queued = false, 100);

        if (Api is ICoreServerAPI) {
            foreach (IServerPlayer player in Api.World.AllOnlinePlayers.Cast<IServerPlayer>()) {
                OnPlayerJoin(player);
            }
        }
    }

    public BitmapRef PingIcon(int ping) {
        return _pingIcon!.Get(Config.Thresholds, ping);
    }

    public override void Dispose() {
        _hud?.Dispose();
        _hud = null;

        _fileWatcher?.Dispose();
        _fileWatcher = null;

        _pingIcon?.Dispose();
        _pingIcon = null;

        if (Api is ICoreServerAPI sapi) {
            sapi.Event.PlayerJoin -= OnPlayerJoin;
        }

        _channel = null;
    }
}
