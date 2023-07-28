using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList;

public class PlayerListMod : ModSystem {
    public override bool ShouldLoad(EnumAppSide side) {
        return side == EnumAppSide.Client;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        _ = new PlayerListHud(api);

        ActionConsumable<KeyCombination> vanillaHandler = api.Input.GetHotKeyByCode("chatdialog").Handler;

        api.Input.SetHotKeyHandler("chatdialog", comb => {
            if (comb.KeyCode == (int)GlKeys.Tab && !comb.Alt && !comb.Ctrl && !comb.Shift) {
                return true;
            } else {
                return vanillaHandler(comb);
            }
        });
    }

    public class PlayerListHud : HudElement {
        private readonly int fontHeight = 25;
        private readonly int width = 200;

        public PlayerListHud(ICoreClientAPI api) : base(api) {
            api.Event.PlayerJoin += UpdateList;
            api.Event.PlayerLeave += UpdateList;

            UpdateList();
        }

        public override void OnRenderGUI(float deltaTime) {
            if (capi.Input.KeyboardKeyState[(int)GlKeys.Tab]) {
                base.OnRenderGUI(deltaTime);
            }
        }

        private void UpdateList(IPlayer notUsed = null) {
            List<string> players = capi.World.AllOnlinePlayers
                .Select(player => player.PlayerName)
                .Order()
                .ToList();

            CairoFont font = new() {
                Color = (double[])GuiStyle.DialogDefaultTextColor.Clone(),
                Fontname = GuiStyle.StandardFontName,
                UnscaledFontsize = GuiStyle.SmallFontSize,
                Orientation = EnumTextOrientation.Center
            };

            GuiComposer composer = capi.Gui
                .CreateCompo("playerlist:thelist", new() {
                    Alignment = EnumDialogArea.CenterTop,
                    BothSizing = ElementSizing.Fixed,
                    fixedWidth = width,
                    fixedHeight = fontHeight * players.Count + fontHeight,
                    fixedOffsetY = 100
                })
                .AddDialogBG(ElementBounds.Fill, false)
                .BeginChildElements();

            int i = -10;
            foreach (string player in players) {
                composer.AddStaticText(player, font, ElementBounds.Fixed(EnumDialogArea.LeftTop, 0, i += fontHeight, width, fontHeight));
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
}
