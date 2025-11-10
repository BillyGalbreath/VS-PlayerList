using playerlist.util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist;

public class PlayerData {
    public string Name { get; }
    public int Ping { get; }
    public CairoFont Font { get; }

    private readonly int _hash;

    public PlayerData(IPlayer player) {
        Name = player.PlayerName;
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
