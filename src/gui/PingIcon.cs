using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace playerlist.gui;

public class PingIcon {
    private readonly BitmapRef[] _icon = new BitmapRef[5];

    public PingIcon(ICoreClientAPI capi) {
        for (int i = 0; i < 5; i++) {
            _icon[i] = capi.Assets.Get(new AssetLocation("playerlist", $"textures/ping_{i}.png")).ToBitmap(capi);
        }
    }

    public BitmapRef Get(int[]? thresholds, int ping) {
        return ping switch {
            _ when ping < 0 || thresholds == null => _icon[0], // 0 bars
            _ when ping < thresholds[0] => _icon[4], // 4 bars
            _ when ping < thresholds[1] => _icon[3], // 3 bars
            _ when ping < thresholds[2] => _icon[2], // 2 bars
            _ => _icon[1] // 1 bar
        };
    }

    public void Dispose() {
        _icon.Foreach(icon => icon.Dispose());
    }
}
