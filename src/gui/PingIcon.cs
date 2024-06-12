using Vintagestory.API.Common;

namespace playerlist.gui;

public abstract class PingIcon {
    private static readonly AssetLocation _unknown = new("playerlist", "textures/ping_0.png");
    private static readonly AssetLocation _bad = new("playerlist", "textures/ping_1.png");
    private static readonly AssetLocation _poor = new("playerlist", "textures/ping_2.png");
    private static readonly AssetLocation _good = new("playerlist", "textures/ping_3.png");
    private static readonly AssetLocation _best = new("playerlist", "textures/ping_4.png");

    public static AssetLocation Get(int[]? thresholds, float ping) {
        int millis = (int)(ping * 1000);
        if (thresholds == null || millis < 0) {
            return _unknown;
        }

        if (millis <= thresholds[0]) {
            return _best;
        }

        if (millis <= thresholds[1]) {
            return _good;
        }

        return millis <= thresholds[2] ? _poor : _bad;
    }
}
