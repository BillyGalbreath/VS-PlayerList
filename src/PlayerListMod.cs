using PlayerList.Hud;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList;

public class PlayerListMod : ModSystem {
    public override bool ShouldLoad(EnumAppSide side) {
        return side == EnumAppSide.Client;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        _ = new PlayerListHud(api);
    }
}
