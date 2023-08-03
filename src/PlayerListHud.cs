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

    public PlayerListHud(KeyHandler keyHandler, ICoreClientAPI api) : base(api) {
        this.keyHandler = keyHandler;

        fontHeight = 25;
        width = 200 - fontHeight;

        capi.Event.PlayerJoin += UpdateList;
        capi.Event.PlayerLeave += UpdateList;

        UpdateList();
    }

    public override void OnRenderGUI(float deltaTime) {
        if (keyHandler.IsKeyComboActive()) {
            base.OnRenderGUI(deltaTime);
        }
    }

    public void UpdateList(IPlayer notUsed = null) {
        List<IPlayer> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName)
            .ToList();

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
