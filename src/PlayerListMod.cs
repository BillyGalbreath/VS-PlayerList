using System.Diagnostics.CodeAnalysis;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList;

[SuppressMessage("ReSharper", "UnusedType.Global")]
public class PlayerListMod : ModSystem {
    private PlayerListHud? _hud;
    private KeyHandler? _keyHandler;

    public override void StartClientSide(ICoreClientAPI api) {
        _keyHandler = new KeyHandler(api);
        _hud = new PlayerListHud(_keyHandler, api);
    }

    public override void Dispose() {
        _hud?.Dispose();
        _keyHandler?.Dispose();
    }
}
