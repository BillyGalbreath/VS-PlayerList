using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace PlayerList;

public class KeyHandler {
    public readonly ICoreClientAPI api;
    private readonly ActionConsumable<KeyCombination> vanillaKeyHandler;

    public KeyHandler(ICoreClientAPI api) {
        this.api = api;

        api.Input.RegisterHotKey("playerlist", Lang.Get("playerlist:keybind-description"), GlKeys.Tab, HotkeyType.GUIOrOtherControls);
        api.Input.SetHotKeyHandler("playerlist", combo => true);

        vanillaKeyHandler = api.Input.GetHotKeyByCode("chatdialog").Handler;

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
                return vanillaKeyHandler(vanillaCombo);
            }
        });
    }

    public void Dispose() {
        api.Input.SetHotKeyHandler("chatdialog", vanillaKeyHandler);
    }

    public bool IsKeyComboActive() {
        KeyCombination combo = api.Input.GetHotKeyByCode("playerlist").CurrentMapping;
        return api.Input.KeyboardKeyState[combo.KeyCode] &&
            IsAltDown() == combo.Alt &&
            IsCtrlDown() == combo.Ctrl &&
            IsShiftDown() == combo.Shift;
    }

    private bool IsAltDown() {
        return api.Input.KeyboardKeyState[(int)GlKeys.AltLeft] ||
            api.Input.KeyboardKeyState[(int)GlKeys.AltRight] ||
            api.Input.KeyboardKeyState[(int)GlKeys.LAlt] ||
            api.Input.KeyboardKeyState[(int)GlKeys.RAlt];
    }

    private bool IsCtrlDown() {
        return api.Input.KeyboardKeyState[(int)GlKeys.ControlLeft] ||
            api.Input.KeyboardKeyState[(int)GlKeys.ControlRight] ||
            api.Input.KeyboardKeyState[(int)GlKeys.LControl] ||
            api.Input.KeyboardKeyState[(int)GlKeys.RControl];
    }

    private bool IsShiftDown() {
        return api.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft] ||
            api.Input.KeyboardKeyState[(int)GlKeys.ShiftRight] ||
            api.Input.KeyboardKeyState[(int)GlKeys.LShift] ||
            api.Input.KeyboardKeyState[(int)GlKeys.RShift];
    }
}
