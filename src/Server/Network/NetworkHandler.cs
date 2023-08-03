using PlayerList.Common.Network;
using Vintagestory.API.Server;

namespace PlayerList.Server.Network;

public class NetworkHandler {
    private readonly ICoreServerAPI api;
    private readonly IServerNetworkChannel channel;

    public NetworkHandler(ICoreServerAPI api) {
        this.api = api;

        channel = api.Network.RegisterChannel("playerlist").RegisterMessageType<PlayerInfoPacket>();
    }

    public void SendPacket(PlayerInfoPacket packet) {
        channel.BroadcastPacket(packet);
    }

    public void Dispose() {
    }
}
