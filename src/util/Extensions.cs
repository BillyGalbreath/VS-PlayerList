﻿using System.Reflection;
using playerlist.gui;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace playerlist.util;

public static class Extensions {
    private const BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Instance;

    public static T? GetField<T>(this object obj, string name) where T : class {
        return obj.GetType().GetField(name, _flags)?.GetValue(obj) as T;
    }

    public static CairoFont EntitlementColoredFont(this IPlayer player) {
        if (player.Entitlements?.Count > 0) {
            if (GlobalConstants.playerColorByEntitlement.TryGetValue(player.Entitlements[0].Code, out double[]? color)) {
                return PlayerList.DefaultFont.Clone().WithColor(color);
            }
        }
        return PlayerList.DefaultFont;
    }

    public static float Ping(this IPlayer player) {
        return player is ClientPlayer clientPlayer ? clientPlayer.Ping : -1;
    }

    public static void AddVtmlText(this GuiComposer composer, string text, CairoFont font, ElementBounds bounds) {
        bounds.fixedX += bounds.fixedPaddingX;
        bounds.fixedY += bounds.fixedPaddingY;
        GuiElementVtmlText element = new(composer.Api, text, font, bounds);
        composer.AddInteractiveElement(element);
        bounds.fixedWidth = element.MaxLineWidth + (bounds.fixedPaddingX * 2);
        bounds.fixedHeight = element.TotalHeight + (bounds.fixedPaddingY * 2);
    }
}
