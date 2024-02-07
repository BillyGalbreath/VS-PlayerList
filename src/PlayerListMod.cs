using System.Diagnostics.CodeAnalysis;
using PlayerList.Configuration;
using PlayerList.Gui;
using PlayerList.Network;
using PlayerList.Util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace PlayerList;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class PlayerListMod : ModSystem {
    public static PlayerListMod Instance { get; private set; } = null!;

    public ICoreAPI? Api { get; private set; }

    private PlayerListHud? _hud;
    private KeyHandler? _keyHandler;
    private NetworkHandler? _networkHandler;

    public PlayerListMod() {
        Instance = this;
    }

    public override void StartPre(ICoreAPI api) {
        Api = api;
        Config.Reload();
    }

    public override void StartClientSide(ICoreClientAPI api) {
        _keyHandler = new KeyHandler(api);
        _hud = new PlayerListHud(_keyHandler, api);
        _networkHandler = new ClientNetworkHandler(this, api);
    }

    public override void StartServerSide(ICoreServerAPI api) {
        _networkHandler = new ServerNetworkHandler(this, api);
    }

    public override void Dispose() {
        _hud?.Dispose();
        _keyHandler?.Dispose();
        _networkHandler?.Dispose();

        Config.Dispose();

        Api = null;
    }
}
