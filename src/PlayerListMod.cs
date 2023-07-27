using PlayerList.Hud;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace PlayerList;

public class PlayerListMod : ModSystem {
    GuiDialog dialog;

    public override bool ShouldLoad(EnumAppSide side) {
        return side == EnumAppSide.Client;
    }

    public override void StartClientSide(ICoreClientAPI api) {
        //api.Event.RegisterRenderer(new PlayerListHud(api), EnumRenderStage.Ortho);

        dialog = new PlayerListDialog(api);

        api.Input.RegisterHotKey("annoyingtextgui", "Annoys you with annoyingly centered text", GlKeys.U, HotkeyType.GUIOrOtherControls);
        api.Input.SetHotKeyHandler("annoyingtextgui", ToggleGui);
    }

    private bool ToggleGui(KeyCombination comb) {
        if (dialog.IsOpened()) dialog.TryClose();
        else dialog.TryOpen();

        return true;
    }
}
