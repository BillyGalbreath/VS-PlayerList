using Vintagestory.API.Common;

namespace PlayerList;

public abstract class PingIcon {
    private static readonly AssetLocation Unknown = new("playerlist", "textures/ping_0.png");
    private static readonly AssetLocation Bad = new("playerlist", "textures/ping_1.png");
    private static readonly AssetLocation Poor = new("playerlist", "textures/ping_2.png");
    private static readonly AssetLocation Good = new("playerlist", "textures/ping_3.png");
    private static readonly AssetLocation Best = new("playerlist", "textures/ping_4.png");

    public static AssetLocation Get(float ping) {
        return ping switch {
            < 0 => Unknown,
            <= 0.065F => Best,
            <= 0.125F => Good,
            <= 0.5F => Poor,
            _ => Bad
        };
    }
}
