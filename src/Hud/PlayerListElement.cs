using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace PlayerList.Hud;

internal class PlayerListElement : HudElement {
    private bool isTabDown;

    public PlayerListElement(ICoreClientAPI api) : base(api) {
        //listenerId = capi.Event.RegisterGameTickListener(OnGameTick, 20);

        ComposeGuis();
    }

    public void ComposeGuis() {
        ElementBounds bounds = new() {
            Alignment = EnumDialogArea.CenterFixed,
            BothSizing = ElementSizing.FitToChildren,
            fixedWidth = 200,
            fixedHeight = 60,
            fixedY = 50
        };

        CairoFont font = new() {
            Color = (double[])GuiStyle.DialogDefaultTextColor.Clone(),
            Fontname = GuiStyle.StandardFontName,
            UnscaledFontsize = GuiStyle.NormalFontSize,
            Orientation = EnumTextOrientation.Center
        };

        Composers["playerlist"] = capi.Gui
            .CreateCompo("playerlist:thelist", bounds)
            .AddShadedDialogBG(ElementBounds.Fill)
            .BeginChildElements(bounds)
                .AddStaticText("Billy", font, ElementBounds.Fixed(0, 0, 200, 20))
                .AddStaticText("Chrysti", font, ElementBounds.Fixed(0, 0, 200, 20))
                .AddStaticText("JoeSchmoe", font, ElementBounds.Fixed(0, 0, 200, 20))
            .EndChildElements()
            .Compose();

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

        //capi.Event.UnregisterGameTickListener(listenerId);
    }
}
