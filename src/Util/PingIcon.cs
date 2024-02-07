using PlayerList.Configuration;
using Vintagestory.API.Common;

namespace PlayerList.Util;

public abstract class PingIcon {
    private static readonly AssetLocation Unknown = new("playerlist", "textures/ping_0.png");
    private static readonly AssetLocation Bad = new("playerlist", "textures/ping_1.png");
    private static readonly AssetLocation Poor = new("playerlist", "textures/ping_2.png");
    private static readonly AssetLocation Good = new("playerlist", "textures/ping_3.png");
    private static readonly AssetLocation Best = new("playerlist", "textures/ping_4.png");

    public static AssetLocation Get(float ping) {
        int[]? thresholds = Config.ServerPingThresholds ?? Config.PingThresholds;

        int millis = (int)(ping * 1000);
        if (thresholds == null || millis < 0) {
            return Unknown;
        }

        if (millis <= thresholds[0]) {
            return Best;
        }

        if (millis <= thresholds[1]) {
            return Good;
        }

        return millis <= thresholds[2] ? Poor : Bad;
    }
}
