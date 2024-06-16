using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace playerlist.gui;

public class GuiElementVtmlText : GuiElementRichtext {
    public GuiElementVtmlText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds)
        : base(capi, VtmlUtil.Richtextify(capi, text, font), bounds) {
        bounds.ParentBounds = ElementBounds.Empty;
        BeforeCalcBounds();
        bounds.ParentBounds = null!;

        bounds.fixedWidth = (MaxLineWidth / RuntimeEnv.GUIScale) + bounds.fixedPaddingX;
        bounds.fixedHeight = (TotalHeight / RuntimeEnv.GUIScale) + (bounds.fixedPaddingY * 2);
    }

    public sealed override void BeforeCalcBounds() {
        base.BeforeCalcBounds();
    }
}
