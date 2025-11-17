using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace playerlist.util;

public class PlayerData {
    public string Name { get; }
    public int Ping { get; }
    public CairoFont Font { get; }

    private readonly int _hash;

    public PlayerData(PlayerList mod, IPlayer player) {
        int max = mod.Config.MaxNameLength ?? 20;
        string name = player.PlayerName;

        Name = name.Length <= max ? name : $"{name[..max]}\u2026";
        Ping = (int)(player.Ping() * 1000);
        Font = player.EntitlementColoredFont();

        _hash = HashCode.Combine(Name, Ping);
    }

    public override int GetHashCode() {
        return _hash;
    }

    public override bool Equals(object? obj) {
        return ReferenceEquals(obj, this) || obj as PlayerData == this;
    }

    public static bool operator ==(PlayerData? a, PlayerData? b) {
        return a is not null && b is not null && a.Name == b.Name && a.Ping == b.Ping;
    }

    public static bool operator !=(PlayerData? a, PlayerData? b) {
        return !(a == b);
    }
}

public static class PlayerExtensions {
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
}
