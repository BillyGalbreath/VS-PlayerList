using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using playerlist.util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public class PlayerListHud : HudElement {
    private readonly PlayerList _mod;
    private readonly KeyHandler _keyHandler;

    private long _gameTickListenerId;
    private bool _wasOpen;

    public PlayerListHud(PlayerList mod) : base((ICoreClientAPI)mod.Api) {
        _mod = mod;

        _keyHandler = new KeyHandler(capi);

        capi.Event.PlayerJoin += UpdateList;
        capi.Event.PlayerLeave += UpdateList;
    }

    public override void OnOwnPlayerDataReceived() {
        UpdateList();
    }

    private void UpdateList(IPlayer? notUsed = null) {
        List<IPlayer> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName) // todo - configurable sort order (maybe?)
            .ToList();

        Composers["playerlist"] = Compose(players).Compose();

        TryOpen();
    }

    private GuiComposer Compose(List<IPlayer> players) {
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
                .WithFixedPosition(GuiPlayerGrid.Padding, headerBounds.fixedHeight + gridBounds.fixedHeight + (GuiPlayerGrid.Padding * 2))
                .WithFixedSize(2048, 0))
            .EndChildElements();
    }

    public override bool ShouldReceiveRenderEvents() {
        bool shouldOpen = _keyHandler.IsKeyComboActive();

        switch (shouldOpen) {
            case true when !_wasOpen:
                UpdateList();
                _gameTickListenerId = capi.Event.RegisterGameTickListener(_ => UpdateList(), 1000);
                break;
            case false when _wasOpen:
                capi.Event.UnregisterGameTickListener(_gameTickListenerId);
                break;
        }

        return _wasOpen = shouldOpen;
    }

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

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
    public override void Dispose() {
        base.Dispose();

        _keyHandler.Dispose();

        capi.Event.UnregisterGameTickListener(_gameTickListenerId);

        capi.Event.PlayerJoin -= UpdateList;
        capi.Event.PlayerLeave -= UpdateList;
    }
}
