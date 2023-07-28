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

        api.Input.RegisterHotKey("playerlist", "Hold to show the player list", GlKeys.Tab, HotkeyType.GUIOrOtherControls);
        api.Input.SetHotKeyHandler("playerlist", combo => true);

        ActionConsumable<KeyCombination> vanillaHandler = api.Input.GetHotKeyByCode("chatdialog").Handler;
        api.Input.SetHotKeyHandler("chatdialog", vanillaCombo => {
            KeyCombination playerlistCombo = api.Input.GetHotKeyByCode("playerlist").CurrentMapping;
            if (vanillaCombo.KeyCode == playerlistCombo.KeyCode &&
                vanillaCombo.SecondKeyCode == playerlistCombo.SecondKeyCode &&
                vanillaCombo.Alt == playerlistCombo.Alt &&
                vanillaCombo.Ctrl == playerlistCombo.Ctrl &&
                vanillaCombo.Shift == playerlistCombo.Shift
            ) {
                return true;
            } else {
                return vanillaHandler(vanillaCombo);
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
