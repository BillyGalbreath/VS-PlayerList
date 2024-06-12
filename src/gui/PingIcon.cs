using Vintagestory.API.Common;

namespace playerlist.gui;

public abstract class PingIcon {
    private static readonly AssetLocation _unknown = new("playerlist", "textures/ping_0.png");
    private static readonly AssetLocation _bad = new("playerlist", "textures/ping_1.png");
    private static readonly AssetLocation _poor = new("playerlist", "textures/ping_2.png");
    private static readonly AssetLocation _good = new("playerlist", "textures/ping_3.png");
    private static readonly AssetLocation _best = new("playerlist", "textures/ping_4.png");

    public static AssetLocation Get(int[]? limits, float ping) {
        int ms = (int)(ping * 1000);
        return limits == null || ms < 0 ? _unknown : ms <= limits[0] ? _best : ms <= limits[1] ? _good : ms <= limits[2] ? _poor : _bad;
    }
}
