﻿using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace playerlist.util;

public static class Extensions {
    private static readonly CairoFont _defaultFont = new() {
        Color = GuiStyle.DialogDefaultTextColor,
        Fontname = GuiStyle.StandardFontName,
        UnscaledFontsize = GuiStyle.SmallFontSize,
        Orientation = EnumTextOrientation.Left
    };

    public static CairoFont EntitlementColoredFont(this IPlayer player) {
        if (player.Entitlements?.Count > 0) {
            if (GlobalConstants.playerColorByEntitlement.TryGetValue(player.Entitlements[0].Code, out double[]? color)) {
                return _defaultFont.Clone().WithColor(color);
            }
        }
        return _defaultFont;
    }

    public static float Ping(this IPlayer player) {
        return player is ClientPlayer clientPlayer ? clientPlayer.Ping : -1;
    }
}
