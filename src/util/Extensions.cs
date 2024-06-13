using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace playerlist.util;

public static class Extensions {
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
}
