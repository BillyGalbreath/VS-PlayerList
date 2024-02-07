using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace PlayerList.Util;

public class KeyHandler {
    private readonly ICoreClientAPI _capi;
    private readonly ActionConsumable<KeyCombination> _vanillaKeyHandler;

    public KeyHandler(ICoreClientAPI capi) {
        _capi = capi;

        capi.Input.RegisterHotKey("playerlist", Lang.Get("playerlist:keybind-description"), GlKeys.Tab, HotkeyType.GUIOrOtherControls);
        capi.Input.SetHotKeyHandler("playerlist", _ => true);

        _vanillaKeyHandler = capi.Input.GetHotKeyByCode("chatdialog").Handler;

        capi.Input.SetHotKeyHandler("chatdialog", vanillaCombo => {
            KeyCombination playerlistCombo = capi.Input.GetHotKeyByCode("playerlist").CurrentMapping;
            if (vanillaCombo.KeyCode == playerlistCombo.KeyCode &&
                vanillaCombo.SecondKeyCode == playerlistCombo.SecondKeyCode &&
                vanillaCombo.Alt == playerlistCombo.Alt &&
                vanillaCombo.Ctrl == playerlistCombo.Ctrl &&
                vanillaCombo.Shift == playerlistCombo.Shift
               ) {
                return true;
            }

            return _vanillaKeyHandler(vanillaCombo);
        });
    }

    public bool IsKeyComboActive() {
        KeyCombination combo = _capi.Input.GetHotKeyByCode("playerlist").CurrentMapping;
        return _capi.Input.KeyboardKeyState[combo.KeyCode] &&
               IsAltDown() == combo.Alt &&
               IsCtrlDown() == combo.Ctrl &&
               IsShiftDown() == combo.Shift;
    }

    private bool IsAltDown() {
        return _capi.Input.KeyboardKeyState[(int)GlKeys.AltLeft] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.AltRight] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.LAlt] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.RAlt];
    }

    private bool IsCtrlDown() {
        return _capi.Input.KeyboardKeyState[(int)GlKeys.ControlLeft] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.ControlRight] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.LControl] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.RControl];
    }

    private bool IsShiftDown() {
        return _capi.Input.KeyboardKeyState[(int)GlKeys.ShiftLeft] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.ShiftRight] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.LShift] ||
               _capi.Input.KeyboardKeyState[(int)GlKeys.RShift];
    }

    public void Dispose() {
        _capi.Input.SetHotKeyHandler("chatdialog", _vanillaKeyHandler);
    }
}
