using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Util;

namespace PlayerList.Client.Hud;

public class PlayerListHud : HudElement {
    private readonly PlayerListClient client;
    private readonly int fontHeight;
    private readonly int width;

    public PlayerListHud(PlayerListClient client) : base(client.API) {
        this.client = client;

        fontHeight = 25;
        width = 200 - fontHeight;

        capi.Event.PlayerJoin += UpdateList;
        capi.Event.PlayerLeave += UpdateList;

        UpdateList();
    }

    public bool IsKeyComboActive() {
        KeyCombination combo = capi.Input.GetHotKeyByCode("playerlist").CurrentMapping;
        return capi.Input.KeyboardKeyState[combo.KeyCode] &&
            IsAltDown() == combo.Alt &&
            IsCtrlDown() == combo.Ctrl &&
            IsShiftDown() == combo.Shift;
    }

    private bool IsAltDown() {
        return capi.Input.KeyboardKeyState[(int)GlKeys.AltLeft] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.AltRight] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.LAlt] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.RAlt];
    }

    private bool IsCtrlDown() {
        return capi.Input.KeyboardKeyState[(int)GlKeys.ControlLeft] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.ControlRight] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.LControl] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.RControl];
    }

    private bool IsShiftDown() {
        return capi.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.ShiftRight] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.LShift] ||
            capi.Input.KeyboardKeyState[(int)GlKeys.RShift];
    }

    public override void OnRenderGUI(float deltaTime) {
        if (IsKeyComboActive()) {
            base.OnRenderGUI(deltaTime);
        }
    }

    public void UpdateList(IPlayer notUsed = null) {
        List<IPlayer> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName)
            .ToList();

        CairoFont font = new() {
            Color = (double[])GuiStyle.DialogDefaultTextColor.Clone(),
            Fontname = GuiStyle.StandardFontName,
            UnscaledFontsize = GuiStyle.SmallFontSize,
            Orientation = EnumTextOrientation.Left
        };

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
            composer.AddStaticText(player.PlayerName, font, ElementBounds.Fixed(EnumDialogArea.LeftTop, fontHeight / 2D, i += fontHeight, width, fontHeight));
            composer.AddImage(ElementBounds.Fixed(EnumDialogArea.LeftTop, width, i, fontHeight, fontHeight), PingIcon.Get(client.Pings.Get(player.PlayerUID, -1)));
        }

        composer.EndChildElements();
        Composers["playerlist"] = composer.Compose();

        TryOpen();
    }

    public override bool ShouldReceiveKeyboardEvents() { return false; }

    public override double InputOrder { get { return 1; } }

    public override bool Focusable => false;

    protected override void OnFocusChanged(bool on) { }

    public override bool TryClose() { return false; }

    public override void Dispose() {
        base.Dispose();

        capi.Event.PlayerJoin -= UpdateList;
        capi.Event.PlayerLeave -= UpdateList;

        GC.SuppressFinalize(this);
    }
}
