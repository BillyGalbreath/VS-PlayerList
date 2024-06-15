using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public class GuiElementVtmlText : GuiElementRichtext {
    public GuiElementVtmlText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds)
        : base(capi, VtmlUtil.Richtextify(capi, text, font), bounds) {
        bounds.ParentBounds = ElementBounds.Empty;
        BeforeCalcBounds();
        bounds.ParentBounds = null!;
    }

    public sealed override void BeforeCalcBounds() {
        base.BeforeCalcBounds();
    }
}
