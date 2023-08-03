using Vintagestory.API.Common;

namespace PlayerList.Client.Hud;

public class PingIcon {
    public static readonly AssetLocation UNKNOWN = new("playerlist", "textures/ping_0.png");
    public static readonly AssetLocation BAD = new("playerlist", "textures/ping_1.png");
    public static readonly AssetLocation POOR = new("playerlist", "textures/ping_2.png");
    public static readonly AssetLocation GOOD = new("playerlist", "textures/ping_3.png");
    public static readonly AssetLocation BEST = new("playerlist", "textures/ping_4.png");

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
