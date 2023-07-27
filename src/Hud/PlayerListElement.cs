using Vintagestory.API.Client;
using Vintagestory.API.Config;

namespace PlayerList.Hud;

internal class PlayerListElement : HudElement {
    private readonly long listenerId;

    private bool tabIsDown;

    public PlayerListElement(ICoreClientAPI api) : base(api) {
        listenerId = capi.Event.RegisterGameTickListener(OnGameTick, 20);

        ComposeGuis();
    }

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
        if (tabIsDown) {
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
        int key = args.KeyCode;
        int? key2 = args.KeyCode2;

        capi.SendChatMessage($"KEY DOWN --> Key: {key} Key2: {key2}");
    }

    public override void OnKeyUp(KeyEvent args) {
        int key = args.KeyCode;
        int? key2 = args.KeyCode2;

        capi.SendChatMessage($"KEY UP --> Key: {key} Key2: {key2}");
    }

    public override bool TryClose() {
        return false;
    }

    public override void Dispose() {
        base.Dispose();

        capi.Event.UnregisterGameTickListener(listenerId);
    }
}
