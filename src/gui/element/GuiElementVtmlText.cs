using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;

namespace playerlist.gui.element;

public class GuiElementVtmlText : GuiElementRichtext {
    public GuiElementVtmlText(ICoreClientAPI capi, string text, CairoFont font, ElementBounds bounds)
        : base(capi, VtmlUtil.Richtextify(capi, text, font), bounds) {
        // start with standard text size
        ElementBounds maxBounds = ElementBounds.Fixed(0, 0);
        font.AutoBoxSize(text, maxBounds);
        bounds.fixedWidth = maxBounds.fixedWidth * 2; // double to compensate for bold, etc

        // calculate real text size
        Bounds.ParentBounds = ElementBounds.Empty;
        CalcHeightAndPositions();
        Bounds.ParentBounds = null!;

        // fix bounds to real size
        Bounds.fixedWidth = MaxLineWidth / RuntimeEnv.GUIScale + 2.0;
        //bounds.fixedHeight = TotalHeight / RuntimeEnv.GUIScale + 2.0;
    }
}

public static class VtmlExtensions {
    public static GuiComposer AddVtmlText(this GuiComposer composer, ICoreClientAPI capi, string? text, CairoFont font, ElementBounds bounds) {
        if (!string.IsNullOrEmpty(text)) {
            composer.AddInteractiveElement(new GuiElementVtmlText(capi, text, font, bounds));
        }

        return composer;
    }
}
