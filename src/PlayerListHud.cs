using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.Client.NoObf;

namespace PlayerList;

public class PlayerListHud : HudElement {
    public static readonly CairoFont DEFAULT_FONT = new() {
        Color = (double[])GuiStyle.DialogDefaultTextColor.Clone(),
        Fontname = GuiStyle.StandardFontName,
        UnscaledFontsize = GuiStyle.SmallFontSize,
        Orientation = EnumTextOrientation.Left
    };

    private readonly KeyHandler keyHandler;
    private readonly int fontHeight;
    private readonly int width;

    private bool wasOpen = false;

    public PlayerListHud(KeyHandler keyHandler, ICoreClientAPI api) : base(api) {
        this.keyHandler = keyHandler;

        fontHeight = 25;
        width = 200 - fontHeight;

        capi.Event.PlayerJoin += UpdateList;
        capi.Event.PlayerLeave += UpdateList;
    }

    public override void OnOwnPlayerDataReceived() {
        UpdateList();
    }

    public void UpdateList(IPlayer notUsed = null) {
        List<IPlayer> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName)
            .ToList();

        Composers["playerlist"] = Compose(players).Compose();

        TryOpen();
    }

    private GuiComposer Compose(List<IPlayer> players) {
        GuiComposer composer = capi.Gui
            .CreateCompo("playerlist:thelist", new() {
                Alignment = EnumDialogArea.CenterTop,
                BothSizing = ElementSizing.Fixed,
                fixedWidth = width + fontHeight,
                fixedHeight = fontHeight * players.Count + fontHeight,
                fixedOffsetY = 100
            })
            .AddDialogBG(ElementBounds.Fill, false)
            .BeginChildElements();

        int i = -10;
        foreach (IPlayer player in players) {
            composer.AddStaticText(
                player.PlayerName,
                player.Entitlements?.Count > 0 && GlobalConstants.playerColorByEntitlement.TryGetValue(player.Entitlements[0].Code, out double[] color) ?
                new() {
                    Color = color,
                    Fontname = GuiStyle.StandardFontName,
                    UnscaledFontsize = GuiStyle.SmallFontSize,
                    Orientation = EnumTextOrientation.Left
                } : DEFAULT_FONT,
                ElementBounds.Fixed(EnumDialogArea.LeftTop, fontHeight / 2D, i += fontHeight, width, fontHeight));
            composer.AddImage(ElementBounds.Fixed(EnumDialogArea.LeftTop, width, i, fontHeight, fontHeight), PingIcon.Get(((ClientPlayer)player).Ping));
        }

        return composer.EndChildElements();
    }

    public override double InputOrder {
        get {
            return 1.05;
        }
    }

    public override double DrawOrder {
        get {
            return 0.88;
        }
    }

    public override float ZSize => 50F;

    public override bool ShouldReceiveRenderEvents() {
        bool shouldOpen = keyHandler.IsKeyComboActive();
        if (shouldOpen && !wasOpen) {
            UpdateList();
        }
        return wasOpen = shouldOpen;
    }

    public override void OnFinalizeFrame(float dt) {
        base.OnFinalizeFrame(dt);
    }

    public override bool ShouldReceiveKeyboardEvents() {
        return true;
    }

    public override void OnKeyDown(KeyEvent args) {
    }

    public override void OnKeyPress(KeyEvent args) {
    }

    public override void OnKeyUp(KeyEvent args) {
    }

    public override bool OnEscapePressed() {
        return false;
    }

    public override bool ShouldReceiveMouseEvents() {
        return true;
    }

    public override void OnMouseDown(MouseEvent args) {
    }

    public override void OnMouseUp(MouseEvent args) {
    }

    public override void OnMouseMove(MouseEvent args) {
    }

    public override void OnMouseWheel(MouseWheelEventArgs args) {
    }

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

    public override void Toggle() {
    }

    public override void UnFocus() {
    }

    public override void Focus() {
    }

    protected override void OnFocusChanged(bool on) {
        focused = false;
    }

    public override void Dispose() {
        base.Dispose();

        capi.Event.PlayerJoin -= UpdateList;
        capi.Event.PlayerLeave -= UpdateList;

        GC.SuppressFinalize(this);
    }
}
