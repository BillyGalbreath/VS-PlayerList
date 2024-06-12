using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Cairo;
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
        // only show the first 100 players
        int maxCount = Math.Min(players.Count, 100);

        int columns = (int)Math.Ceiling(maxCount / 25D);
        int rows = (int)Math.Ceiling(maxCount / (double)columns);

        int rowHeight = (int)Math.Ceiling(GuiStyle.SmallFontSize + (GuiStyle.SmallFontSize / 2D));
        int columnWidth = 100;

        // calculate max player name width
        for (int i = 0; i < maxCount; i++) {
            IPlayer player = players[i];
            TextExtents extents = player.EntitlementColoredFont().GetTextExtents(player.PlayerName);
            columnWidth = (int)Math.Max(extents.Width + (rowHeight * 2), columnWidth);
        }

        // todo - add configurable header and footer for servers
        // todo - show cur/max player counts
        GuiComposer composer = capi.Gui
            .CreateCompo("playerlist:thelist", new ElementBounds {
                Alignment = EnumDialogArea.CenterTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = columnWidth * columns,
                fixedHeight = rowHeight * rows,
                fixedOffsetY = 100
            })
            .AddDialogBG(ElementBounds.Fill, false)
            .BeginChildElements();

        for (int i = 0; i < maxCount; i++) {
            ElementBounds bounds = new() {
                Alignment = EnumDialogArea.LeftTop,
                BothSizing = ElementSizing.Fixed,
                // ReSharper disable once PossibleLossOfFraction
                fixedX = rowHeight + (rowHeight / 2D) + columnWidth * (i / rows),
                fixedY = rowHeight * (i % rows),
                fixedHeight = rowHeight,
                fixedWidth = columnWidth - rowHeight
            };
            composer.AddStaticTextAutoFontSize(players[i].PlayerName, players[i].EntitlementColoredFont(), bounds);
            composer.AddImage(bounds.CopyOffsetedSibling(-rowHeight, 4), PingIcon.Get(_mod.Thresholds, players[i].Ping()));
        }

        return composer.EndChildElements();
    }

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
