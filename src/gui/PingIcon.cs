using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public class PingIcon {
    private readonly BitmapRef _0;
    private readonly BitmapRef _1;
    private readonly BitmapRef _2;
    private readonly BitmapRef _3;
    private readonly BitmapRef _4;

    public PingIcon(ICoreClientAPI capi) {
        _0 = capi.Assets.Get(new AssetLocation("playerlist", "textures/ping_0.png")).ToBitmap(capi);
        _1 = capi.Assets.Get(new AssetLocation("playerlist", "textures/ping_1.png")).ToBitmap(capi);
        _2 = capi.Assets.Get(new AssetLocation("playerlist", "textures/ping_2.png")).ToBitmap(capi);
        _3 = capi.Assets.Get(new AssetLocation("playerlist", "textures/ping_3.png")).ToBitmap(capi);
        _4 = capi.Assets.Get(new AssetLocation("playerlist", "textures/ping_4.png")).ToBitmap(capi);
    }

    public BitmapRef Get(int[]? thresholds, int ping) {
        if (thresholds == null || ping < 0) {
            return _0;
        }

        if (ping <= thresholds[0]) {
            return _4;
        }

        if (ping <= thresholds[1]) {
            return _3;
        }

        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (ping <= thresholds[2]) {
            return _2;
        }

        return _1;
    }

    public void Dispose() {
        _0.Dispose();
        _1.Dispose();
        _2.Dispose();
        _3.Dispose();
        _4.Dispose();
    }
}
