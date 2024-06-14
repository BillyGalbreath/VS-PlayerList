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
    public static readonly CairoFont DefaultFont = new() {
        Color = GuiStyle.DialogDefaultTextColor,
        Fontname = GuiStyle.StandardFontName,
        UnscaledFontsize = GuiStyle.SmallFontSize,
        Orientation = EnumTextOrientation.Left
    };

    public ICoreAPI Api { get; private set; } = null!;

    public ILogger Logger => Mod.Logger;
    public string ModId => Mod.Info.ModID;

    private Config? _config;
    private FileWatcher? _fileWatcher;
    private PlayerListHud? _hud;
    private IServerNetworkChannel? _channel;
    private PingIcon? _pingIcon;
    private int[]? _serverThresholds;

    public override void StartPre(ICoreAPI api) {
        Api = api;
        ReloadConfig();
    }

    public override void StartClientSide(ICoreClientAPI capi) {
        _hud = new PlayerListHud(this);
        _pingIcon = new PingIcon(capi);

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
        GamePaths.EnsurePathExists(GamePaths.ModConfig);

        _config = Api.LoadModConfig<Config>($"{ModId}.json") ?? new Config();

        string json = JsonConvert.SerializeObject(_config, new JsonSerializerSettings {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Include,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });

        (_fileWatcher ??= new FileWatcher(this)).Queued = true;

        File.WriteAllText(Path.Combine(GamePaths.ModConfig, $"{ModId}.json"), json);

        Api.Event.RegisterCallback(_ => _fileWatcher.Queued = false, 100);
    }

    public BitmapRef PingIcon(float ping) {
        return _pingIcon!.Get(_serverThresholds ?? (_config ?? new Config()).Thresholds, ping);
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
