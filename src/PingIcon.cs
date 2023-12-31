using Vintagestory.API.Common;

namespace PlayerList;

public abstract class PingIcon {
    private static readonly AssetLocation UNKNOWN = new("playerlist", "textures/ping_0.png");
    private static readonly AssetLocation BAD = new("playerlist", "textures/ping_1.png");
    private static readonly AssetLocation POOR = new("playerlist", "textures/ping_2.png");
    private static readonly AssetLocation GOOD = new("playerlist", "textures/ping_3.png");
    private static readonly AssetLocation BEST = new("playerlist", "textures/ping_4.png");

    public static AssetLocation Get(float ping) {
        return ping switch {
            < 0 => UNKNOWN,
            <= 0.065F => BEST,
            <= 0.125F => GOOD,
            <= 0.5F => POOR,
            _ => BAD,
        };
    }
}
