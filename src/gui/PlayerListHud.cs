using playerlist.util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public sealed class PlayerListHud : HudElement {
    private readonly PlayerList _mod;
    private readonly KeyHandler _keyHandler;
    private readonly long _gameTickListenerId;

    private List<string> _players = [];

    public PlayerListHud(PlayerList mod) : base((ICoreClientAPI)mod.Api) {
        _mod = mod;
        _keyHandler = new KeyHandler(capi);
        _gameTickListenerId = capi.Event.RegisterGameTickListener(_ => UpdateList(), 1000);
    }

    private void UpdateList() {
        // quickly get players
        List<string> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName)
            .Select(player => player.PlayerUID)
            .ToList();

        // for testing
        for (int i = 0; i < 20; i++) players.Add(players[0]);

        if (_players.SequenceEqual(players)) {
            // nothing changed
            return;
        }

        SingleComposer = Compose(_players = players).Compose();

        TryOpen();
    }

    private GuiComposer Compose(List<string> players) {
        (string? header, string? footer) = Util.ParseHeaderAndFooter(_mod, players.Count);

        ElementBounds dialog = new() {
            Alignment = EnumDialogArea.CenterTop,
            BothSizing = ElementSizing.FitToChildren,
            fixedOffsetY = 100
        };
        ElementBounds bgBounds = new() {
            Alignment = EnumDialogArea.CenterTop,
            BothSizing = ElementSizing.FitToChildren,
            percentWidth = 1.0,
            percentHeight = 1.0,
            fixedPaddingX = GuiPlayerGrid.Padding,
            fixedPaddingY = GuiPlayerGrid.Padding
        };
        ElementBounds gridBounds = new() {
            Alignment = EnumDialogArea.CenterTop,
            BothSizing = ElementSizing.Fixed
        };
        ElementBounds headerBounds = new() {
            Alignment = EnumDialogArea.CenterTop,
            BothSizing = ElementSizing.Fixed,
            fixedX = GuiPlayerGrid.Padding,
            fixedWidth = 2048,
            fixedPaddingX = GuiPlayerGrid.Padding,
            fixedPaddingY = GuiPlayerGrid.Padding
        };

        return capi.Gui
            .CreateCompo("playerlist", dialog)
            .AddGameOverlay(bgBounds, GuiStyle.DialogDefaultBgColor)
            .BeginChildElements(bgBounds)
            .AddVtmlText(capi, header, Util.CenteredFont, headerBounds)
            .AddStaticElement(new GuiPlayerGrid(_mod, players, gridBounds
                .WithFixedOffset(0, headerBounds.fixedHeight)))
            .AddVtmlText(capi, footer, Util.CenteredFont, headerBounds.FlatCopy()
                .WithFixedPosition(GuiPlayerGrid.Padding, headerBounds.fixedHeight + gridBounds.fixedHeight + GuiPlayerGrid.Padding)
                .WithFixedSize(2048, 0))
            .EndChildElements();
    }

    public override bool ShouldReceiveRenderEvents() {
        return _keyHandler.IsKeyComboActive();
    }

    // @formatter:off
    public override double InputOrder => 1.0999;
    public override double DrawOrder => 0.8899;
    public override float ZSize => 200F;
    public override bool ShouldReceiveKeyboardEvents() => false;
    public override void OnKeyDown(KeyEvent args) { }
    public override void OnKeyPress(KeyEvent args) { }
    public override void OnKeyUp(KeyEvent args) { }
    public override bool OnEscapePressed() => false;
    public override bool ShouldReceiveMouseEvents() => false;
    public override void OnMouseDown(MouseEvent args) { }
    public override void OnMouseUp(MouseEvent args) { }
    public override void OnMouseMove(MouseEvent args) { }
    public override void OnMouseWheel(MouseWheelEventArgs args) { }
    public override bool OnMouseEnterSlot(ItemSlot slot) => false;
    public override bool OnMouseLeaveSlot(ItemSlot itemSlot) => false;
    public override bool CaptureAllInputs() => false;
    public override bool TryClose() => false;
    public override void Toggle() { }
    public override void UnFocus() { }
    public override void Focus() { }
    public override bool Focused => false;
    protected override void OnFocusChanged(bool on) => focused = false;
    // @formatter:on

    public override void Dispose() {
        base.Dispose();

        _keyHandler.Dispose();

        capi.Event.UnregisterGameTickListener(_gameTickListenerId);
    }
}
