using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace PlayerList.Hud;

internal class PlayerListElement : HudElement {
    private bool isTabDown;

    public PlayerListElement(ICoreClientAPI api) : base(api) {
        capi.Event.PlayerJoin += OnPlayerJoin;
        capi.Event.PlayerLeave += OnPlayerLeave;

        ComposeGuis();
    }

    private void OnPlayerJoin(IPlayer player) {
        ComposeGuis();
    }

    private void OnPlayerLeave(IPlayer player) {
        ComposeGuis();
    }

    private void ComposeGuis() {
        List<string> players = new();

        foreach (IPlayer player in capi.World.AllOnlinePlayers) {
            players.Add(player.PlayerName);
        }

        /*players.Add("Billy");
        players.Add("Chrysti");
        players.Add("JoeSchmoe");
        players.Add("kaii");
        players.Add("BlazeAttack");
        players.Add("Lazer2004");
        players.Add("fedoratheexplorer");*/

        players.Sort();

        int i = -10;
        int fontHeight = 20;
        int width = 200;

        CairoFont font = new() {
            Color = (double[])GuiStyle.DialogDefaultTextColor.Clone(),
            Fontname = GuiStyle.StandardFontName,
            UnscaledFontsize = GuiStyle.SmallFontSize,
            Orientation = EnumTextOrientation.Center
        };

        GuiComposer composer = capi.Gui
            .CreateCompo("playerlist:thelist", new() {
                Alignment = EnumDialogArea.CenterTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = width,
                fixedHeight = fontHeight * players.Count + fontHeight,
                fixedOffsetY = 100
            })
            .AddDialogBG(ElementBounds.Fill, false)
            .BeginChildElements();
        foreach (string player in players) {
            composer.AddStaticText(player, font, ElementBounds.Fixed(EnumDialogArea.LeftTop, 0, i += 20, width, fontHeight));
        }
        composer.EndChildElements();
        Composers["playerlist"] = composer.Compose();

        TryOpen();
    }

    public override void OnRenderGUI(float deltaTime) {
        if (isTabDown) {
            base.OnRenderGUI(deltaTime);
        }
    }

    public override string ToggleKeyCombinationCode { get { return null; } }

    public override double InputOrder { get { return 1; } }

    public override bool Focusable => false;

    protected override void OnFocusChanged(bool on) {
    }

    public override bool ShouldReceiveKeyboardEvents() {
        return true;
    }

    public override void OnKeyDown(KeyEvent args) {
        if (args.KeyCode == 52 || args.KeyCode2 == 52) {
            isTabDown = true;
        }
    }

    public override void OnKeyUp(KeyEvent args) {
        if (args.KeyCode == 52 || args.KeyCode2 == 52) {
            isTabDown = false;
        }
    }

    public override bool TryClose() {
        return false;
    }

    public override void Dispose() {
        base.Dispose();

        capi.Event.PlayerJoin -= OnPlayerJoin;
        capi.Event.PlayerLeave -= OnPlayerLeave;
    }
}
