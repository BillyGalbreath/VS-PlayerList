using PlayerList.Common.Network;
using PlayerList.Server.Network;
using System.Linq;
using Vintagestory.API.Server;

namespace PlayerList.Server;

public class PlayerListServer {
    private readonly ICoreServerAPI api;
    private readonly NetworkHandler networkHandler;
    private readonly long taskId;

    public PlayerListServer(ICoreServerAPI api) {
        this.api = api;

        networkHandler = new NetworkHandler(api);

        taskId = api.Event.RegisterGameTickListener(Update, 5000, 0);
    }

    private void Update(float deltaTime) {
        PlayerInfoPacket packet = new();
        foreach (IServerPlayer player in api.World.AllOnlinePlayers.Cast<IServerPlayer>()) {
            packet.pings.Add(player.PlayerUID, player.Ping);
        }
        api.Logger.Event("Sending packet: " + packet);
        if (packet == null) {
            api.Logger.Event("Null packet");
            return;
        }
        if (packet.pings == null) {
            api.Logger.Event("Null pings");
            return;
        }
        if (packet.pings.Count == 0) {
            api.Logger.Event("Empty pings");
            return;
        }
        if (networkHandler == null) {
            api.Logger.Event("Null networkHandler");
            return;
        }
        networkHandler.SendPacket(packet);
    }

    public void Dispose() {
        api.Event.UnregisterGameTickListener(taskId);
        networkHandler.Dispose();
    }
}
