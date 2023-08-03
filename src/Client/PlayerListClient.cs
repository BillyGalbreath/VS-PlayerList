using PlayerList.Client.Hud;
using PlayerList.Client.Network;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList.Client;

public class PlayerListClient {
    private readonly ICoreClientAPI api;
    private readonly NetworkHandler networkHandler;
    private readonly PlayerListHud hud;
    private readonly ActionConsumable<KeyCombination> vanillaHandler;

    private Dictionary<string, float> pings = new();

    public ICoreClientAPI API { get { return api; } }
    public PlayerListHud Hud { get { return hud; } }

    public Dictionary<string, float> Pings {
        get {
            return pings;
        }
        set {
            pings = value;
        }
    }

    public PlayerListClient(ICoreClientAPI api) {
        this.api = api;

        networkHandler = new NetworkHandler(this);

        hud = new PlayerListHud(this);

        api.Input.RegisterHotKey("playerlist", "Hold to show the player list", GlKeys.Tab, HotkeyType.GUIOrOtherControls);
        api.Input.SetHotKeyHandler("playerlist", combo => true);

        vanillaHandler = api.Input.GetHotKeyByCode("chatdialog").Handler;

        api.Input.SetHotKeyHandler("chatdialog", vanillaCombo => {
            KeyCombination playerlistCombo = api.Input.GetHotKeyByCode("playerlist").CurrentMapping;
            if (vanillaCombo.KeyCode == playerlistCombo.KeyCode &&
                vanillaCombo.SecondKeyCode == playerlistCombo.SecondKeyCode &&
                vanillaCombo.Alt == playerlistCombo.Alt &&
                vanillaCombo.Ctrl == playerlistCombo.Ctrl &&
                vanillaCombo.Shift == playerlistCombo.Shift
            ) {
                return true;
            } else {
                return vanillaHandler(vanillaCombo);
            }
        });
    }

    public void Dispose() {
        networkHandler.Dispose();

        hud.Dispose();

        api.Input.SetHotKeyHandler("chatdialog", vanillaHandler);
    }
}
