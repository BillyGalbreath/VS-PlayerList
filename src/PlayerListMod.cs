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
    public Config Config { get; private set; } = null!;

    public ILogger Logger => Mod.Logger;
    public string ModId => Mod.Info.ModID;

    private FileWatcher FileWatcher => _fileWatcher ??= new FileWatcher(this);

    private PlayerListHud? _hud;
    private FileWatcher? _fileWatcher;
    private IServerNetworkChannel? _channel;

    public override void StartPre(ICoreAPI api) {
        Api = api;
        ReloadConfig();
    }

    public override void StartClientSide(ICoreClientAPI api) {
        _hud = new PlayerListHud(this);

        api.Network.RegisterChannel(Mod.Info.ModID)
            .RegisterMessageType<Config>()
            .SetMessageHandler<Config>(packet => {
                Mod.Logger.Event($"Received ping thresholds of {string.Join(",", packet.Thresholds!)} from server");
                Config.Thresholds = packet.Thresholds;
            });
    }

    public override void StartServerSide(ICoreServerAPI api) {
        _channel = api.Network.RegisterChannel(Mod.Info.ModID)
            .RegisterMessageType<Config>()
            .SetMessageHandler<Config>((_, _) => { });

        api.Event.PlayerJoin += OnPlayerJoin;
    }

    private void OnPlayerJoin(IServerPlayer player) {
        _channel?.SendPacket(Config, player);
    }

    public void ReloadConfig() {
        Config = Api.LoadModConfig<Config>($"{ModId}.json") ?? new Config();

        FileWatcher.Queued = true;

        string json = JsonConvert.SerializeObject(Config, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        FileInfo fileInfo = new(Path.Combine(GamePaths.ModConfig, $"{ModId}.json"));
        GamePaths.EnsurePathExists(fileInfo.Directory!.FullName);
        File.WriteAllText(fileInfo.FullName, json);
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
