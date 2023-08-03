using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList;

public class PlayerListMod : ModSystem {
    private PlayerListHud hud;
    private KeyHandler keyHandler;

    public override bool AllowRuntimeReload => true;

    public override bool ShouldLoad(EnumAppSide forSide) {
        return forSide.IsClient();
    }

    public override void StartClientSide(ICoreClientAPI api) {
        keyHandler = new KeyHandler(api);
        hud = new PlayerListHud(keyHandler, api);
    }

    public override double ExecuteOrder() {
        return 0.3;
    }

    public override void Dispose() {
        hud.Dispose();
        keyHandler.Dispose();
    }
}
