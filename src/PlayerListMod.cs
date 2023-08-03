using PlayerList.Client;
using PlayerList.Server;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace PlayerList;

public class PlayerListMod : ModSystem {
    private PlayerListClient client;
    private PlayerListServer server;

    public override bool AllowRuntimeReload => true;

    public override void StartClientSide(ICoreClientAPI api) {
        client = new PlayerListClient(api);
    }

    public override void StartServerSide(ICoreServerAPI api) {
        server = new PlayerListServer(api);
    }

    public override double ExecuteOrder() {
        return 0.3;
    }

    public override void Dispose() {
        client?.Dispose();
        server?.Dispose();
    }
}
