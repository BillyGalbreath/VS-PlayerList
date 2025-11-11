using System.Reflection;
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
                return Util.DefaultFont.Clone().WithColor(color);
            }
        }

        return Util.DefaultFont;
    }

    public static float Ping(this IPlayer player) {
        return player is ClientPlayer clientPlayer ? clientPlayer.Ping : -1;
    }

    public static GuiComposer TryAddStaticElement(this GuiComposer composer, GuiElement? element) {
        if (element != null) {
            composer.AddStaticElement(element);
        }

        return composer;
    }

    public static GuiComposer AddVtmlText(this GuiComposer composer, ICoreClientAPI capi, string? text, CairoFont font, ElementBounds bounds) {
        if (!string.IsNullOrEmpty(text)) {
            composer.AddInteractiveElement(new GuiElementVtmlText(capi, text, font, bounds));
        }

        return composer;
    }
}
