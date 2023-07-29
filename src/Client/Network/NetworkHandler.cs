using PlayerList.Common.Network;

namespace PlayerList.Client.Network;

public class NetworkHandler {
    private readonly PlayerListClient client;

    public NetworkHandler(PlayerListClient client) {
        this.client = client;

        client.API.Network.RegisterChannel("playerlist")
            .RegisterMessageType<PlayerInfoPacket>()
            .SetMessageHandler<PlayerInfoPacket>(ReceivePacket);
    }

    private void ReceivePacket(PlayerInfoPacket packet) {
        client.Pings = packet.pings;
        client.Hud.UpdateList();
    }

    public void Dispose() {
    }
}
