using Cairo;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public class GuiElementScalableImage : GuiElementTextBase {
    private readonly BitmapRef _bmp;

    public GuiElementScalableImage(ICoreClientAPI capi, ElementBounds bounds, BitmapRef bmp) : base(capi, "", null!, bounds) {
        _bmp = bmp;
    }

    public override void ComposeElements(Context context, ImageSurface surface) {
        Bounds.CalcWorldBounds();
        surface.Image(_bmp, (int)Bounds.drawX, (int)Bounds.drawY, (int)Bounds.InnerWidth, (int)Bounds.InnerHeight);
    }
}
