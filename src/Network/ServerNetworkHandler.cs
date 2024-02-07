using PlayerList.Configuration;
using PlayerList.Network.Packet;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace PlayerList.Network;

public class ServerNetworkHandler : NetworkHandler {
    private readonly ICoreServerAPI _sapi;

    private IServerNetworkChannel? _channel;

    public ServerNetworkHandler(ModSystem mod, ICoreServerAPI sapi) {
        _sapi = sapi;

        _channel = sapi.Network.RegisterChannel(mod.Mod.Info.ModID)
            .RegisterMessageType<SyncConfigPacket>()
            .SetMessageHandler<SyncConfigPacket>((_, _) => { });

        sapi.Event.PlayerJoin += OnPlayerJoin;
    }

    private void OnPlayerJoin(IServerPlayer player) {
        _channel?.SendPacket(new SyncConfigPacket {
            Thresholds = Config.PingThresholds
        }, player);
    }

    public override void Dispose() {
        _sapi.Event.PlayerJoin -= OnPlayerJoin;

        _channel = null;
    }
}
