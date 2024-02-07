using PlayerList.Configuration;
using PlayerList.Network.Packet;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList.Network;

public class ClientNetworkHandler : NetworkHandler {
    public ClientNetworkHandler(ModSystem mod, ICoreClientAPI capi) {
        capi.Network.RegisterChannel(mod.Mod.Info.ModID)
            .RegisterMessageType<SyncConfigPacket>()
            .SetMessageHandler<SyncConfigPacket>(packet => {
                mod.Mod.Logger.Event($"Received ping thresholds of {string.Join(",", packet.Thresholds!)} from server");
                Config.ServerPingThresholds = packet.Thresholds;
            });
    }

    public override void Dispose() { }
}
