using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace PlayerList.Hud;

internal class PlayerListElement : HudElement {
    readonly long listenerId;

    public override double InputOrder { get { return 1; } }

    public PlayerListElement(ICoreClientAPI api) : base(api) {
        listenerId = capi.Event.RegisterGameTickListener(OnGameTick, 20);

        ComposeGuis();
    }

    public override string ToggleKeyCombinationCode { get { return null; } }

    private void OnGameTick(float dt) {
        Update();
    }


    void Update() {
        //
    }

    public void ComposeGuis() {
        ElementBounds bounds = new ElementBounds() {
            Alignment = EnumDialogArea.CenterFixed,
            BothSizing = ElementSizing.Fixed,
            fixedWidth = 850,
            fixedHeight = 50,
            fixedY = 10
        }.WithFixedAlignmentOffset(0, 5);

        Composers["playerlist"] = capi.Gui
            .CreateCompo("playerlist:thelist", bounds.FlatCopy().FixedGrow(0, 20))
            .BeginChildElements(bounds)
                    .AddStaticText(Lang.Get("test"), CairoFont.WhiteSmallText(), ElementBounds.Fixed(0, 0, 200, 20))
            .EndChildElements()
            .Compose();

        TryOpen();
    }

    public override void OnRenderGUI(float deltaTime) {
        base.OnRenderGUI(deltaTime);
    }

    public override bool Focusable => false;

    protected override void OnFocusChanged(bool on) {
    }

    public override bool ShouldReceiveKeyboardEvents() {
        return false;
    }

    public override bool TryClose() {
        return base.TryClose();
    }

    public override void Dispose() {
        base.Dispose();

        capi.Event.UnregisterGameTickListener(listenerId);
    }
}
