using playerlist.util;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public sealed class PlayerListHud : HudElement {
    private readonly PlayerList _mod;
    private readonly KeyHandler _keyHandler;
    private readonly long _gameTickListenerId;

    private List<string> _players = [];

    private HeaderImage? _logoImage;

    public PlayerListHud(PlayerList mod) : base((ICoreClientAPI)mod.Api) {
        _mod = mod;
        _keyHandler = new KeyHandler(capi);
        _gameTickListenerId = capi.Event.RegisterGameTickListener(_ => UpdateList(), 1000);
    }

    public void UpdateList(bool force = false) {
        // quickly get players
        List<string> players = capi.World.AllOnlinePlayers
            .OrderBy(player => player.PlayerName)
            .Select(player => player.PlayerUID)
            .ToList();

        // check if we need to update
        if (!force && _players.SequenceEqual(players)) {
            return; // nothing changed
        }

        // debug testing stuff
        /*if (players.Count > 0) {
            for (int i = 0; i < 100; i++) {
                players.Add(players[0]);
            }
        }*/

        // build logo image only once, not on every compose
        if (force || _logoImage == null) {
            if (string.IsNullOrEmpty(_mod.Config.Logo)) {
                _logoImage = null;
            } else {
                try {
                    _logoImage = new HeaderImage(_mod, _mod.Config.Logo, ElementBounds.Empty);
                } catch (Exception) {
                    _logoImage = null;
                }
            }
        }

        // build it
        Compose(_players = players);

        // ensure gui is always open
        TryOpen();
    }

    private void Compose(List<string> players) {
        if (players.Count == 0) {
            return;
        }

        (string? header, string? footer) = Util.ParseHeaderAndFooter(_mod, players.Count);

        ElementBounds logo = new() {
            Alignment = EnumDialogArea.CenterTop,
            BothSizing = ElementSizing.Fixed
        };
        ElementBounds headText = logo.FlatCopy();
        ElementBounds gridList = logo.FlatCopy();
        ElementBounds footText = logo.FlatCopy();

        SingleComposer = capi.Gui
            .CreateCompo("playerlist", new ElementBounds {
                Alignment = EnumDialogArea.CenterTop,
                BothSizing = ElementSizing.FitToChildren,
                fixedOffsetY = 100
            })
            .AddGameOverlay(new ElementBounds {
                    Alignment = EnumDialogArea.CenterTop,
                    BothSizing = ElementSizing.FitToChildren
                }.WithFixedPadding(GuiPlayerGrid.Padding),
                GuiStyle.DialogDefaultBgColor)
            .BeginChildElements()
            .TryAddStaticElement(_logoImage?.SetBounds(logo))
            .AddVtmlText(capi, header, Util.CenteredFont, headText.FixedUnder(logo, _logoImage != null ? GuiPlayerGrid.Padding : 0))
            .AddStaticElement(new GuiPlayerGrid(_mod, players, gridList.FixedUnder(headText, !string.IsNullOrEmpty(header) ? GuiPlayerGrid.Padding : 0)))
            .AddVtmlText(capi, footer, Util.CenteredFont, footText.FixedUnder(gridList, GuiPlayerGrid.Padding))
            .EndChildElements()
            .Compose();
    }

    public override bool ShouldReceiveRenderEvents() {
        // only draw when key combo is active (tab pressed)
        return _keyHandler.IsKeyComboActive();
    }

    // @formatter:off
    public override double InputOrder => 1.0999; // right behind chat dialog
    public override double DrawOrder => 0.8899; // right behind escape menu
    public override float ZSize => 200F; // push to top

    // do not listen to any inputs
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

    // do not close or take focus
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
