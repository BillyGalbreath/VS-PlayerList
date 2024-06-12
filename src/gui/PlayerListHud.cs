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
    private readonly int _fontHeight;
    private readonly int _width;

    private long _gameTickListenerId;
    private bool _wasOpen;

    public PlayerListHud(PlayerList mod) : base((ICoreClientAPI)mod.Api) {
        _mod = mod;

        _keyHandler = new KeyHandler(capi);
        _fontHeight = 25;
        _width = 250 - _fontHeight;

        capi.Event.PlayerJoin += UpdateList;
        capi.Event.PlayerLeave += UpdateList;
    }

    public override void OnOwnPlayerDataReceived() {
        UpdateList();
    }

    private void UpdateList(IPlayer? notUsed = null) {
        List<IPlayer> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName)
            .ToList();

        Composers["playerlist"] = Compose(players).Compose();

        TryOpen();
    }

    private GuiComposer Compose(List<IPlayer> players) {
        GuiComposer composer = capi.Gui
            .CreateCompo("playerlist:thelist", new ElementBounds {
                Alignment = EnumDialogArea.CenterTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = _width + _fontHeight,
                fixedHeight = _fontHeight * players.Count + _fontHeight,
                fixedOffsetY = 100
            })
            .AddDialogBG(ElementBounds.Fill, false)
            .BeginChildElements();

        int i = -10;
        foreach (IPlayer player in players) {
            composer.AddStaticText(
                player.PlayerName,
                player.EntitlementColoredFont(),
                ElementBounds.Fixed(EnumDialogArea.LeftTop, _fontHeight / 2D, i += _fontHeight, _width, _fontHeight)
            );
            AssetLocation pingIcon = PingIcon.Get(_mod.Config.Thresholds, player.Ping());
            composer.AddImage(ElementBounds.Fixed(EnumDialogArea.LeftTop, _width, i, _fontHeight, _fontHeight), pingIcon);
        }

        return composer.EndChildElements();
    }

    public override double InputOrder => 1.0999;

    public override double DrawOrder => 0.8899;

    public override float ZSize => 200F;

    public override bool ShouldReceiveRenderEvents() {
        bool shouldOpen = _keyHandler.IsKeyComboActive();

        switch (shouldOpen) {
            case true when !_wasOpen:
                _gameTickListenerId = capi.Event.RegisterGameTickListener(_ => UpdateList(), 1000);
                break;
            case false when _wasOpen:
                capi.Event.UnregisterGameTickListener(_gameTickListenerId);
                break;
        }

        return _wasOpen = shouldOpen;
    }

    public override bool ShouldReceiveKeyboardEvents() {
        return false;
    }

    public override void OnKeyDown(KeyEvent args) { }

    public override void OnKeyPress(KeyEvent args) { }

    public override void OnKeyUp(KeyEvent args) { }

    public override bool OnEscapePressed() {
        return false;
    }

    public override bool ShouldReceiveMouseEvents() {
        return false;
    }

    public override void OnMouseDown(MouseEvent args) { }

    public override void OnMouseUp(MouseEvent args) { }

    public override void OnMouseMove(MouseEvent args) { }

    public override void OnMouseWheel(MouseWheelEventArgs args) { }

    public override bool OnMouseEnterSlot(ItemSlot slot) {
        return false;
    }

    public override bool OnMouseLeaveSlot(ItemSlot itemSlot) {
        return false;
    }

    public override bool CaptureAllInputs() {
        return false;
    }

    public override bool TryClose() {
        return false;
    }

    public override void Toggle() { }

    public override void UnFocus() { }

    public override void Focus() { }

    public override bool Focused => false;

    protected override void OnFocusChanged(bool on) {
        focused = false;
    }

    [SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
    public override void Dispose() {
        base.Dispose();

        _keyHandler.Dispose();

        capi.Event.UnregisterGameTickListener(_gameTickListenerId);

        capi.Event.PlayerJoin -= UpdateList;
        capi.Event.PlayerLeave -= UpdateList;
    }
}
