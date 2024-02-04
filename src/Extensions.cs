using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace PlayerList;

public static class Extensions {
    private static readonly CairoFont DefaultFont = new() {
        Color = GuiStyle.DialogDefaultTextColor,
        Fontname = GuiStyle.StandardFontName,
        UnscaledFontsize = GuiStyle.SmallFontSize,
        Orientation = EnumTextOrientation.Left
    };

    public static CairoFont EntitlementColoredFont(this IPlayer player) {
        return player.Entitlements?.Count > 0 && GlobalConstants.playerColorByEntitlement.TryGetValue(player.Entitlements[0].Code, out double[]? color) ? DefaultFont.Clone().WithColor(color) : DefaultFont;
    }

    public static float Ping(this IPlayer player) {
        return player is ClientPlayer clientPlayer ? clientPlayer.Ping : 0;
    }
}
