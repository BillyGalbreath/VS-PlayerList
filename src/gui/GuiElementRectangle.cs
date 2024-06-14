using Cairo;
using Vintagestory.API.Client;

namespace playerlist.gui;

public class GuiElementRectangle : GuiElementTextBase {
    private static readonly double[] _clear = { 0, 0, 0, 0 };

    private readonly double[] _fill;

    public GuiElementRectangle(ICoreClientAPI api, ElementBounds bounds, double[] fill) : base(api, "", null!, bounds) {
        _fill = fill;
    }

    public override void ComposeElements(Context context, ImageSurface surface) {
        Bounds.CalcWorldBounds();

        context.SetSourceRGBA(_fill);
        context.Rectangle(Bounds.drawX, Bounds.drawY, Bounds.OuterWidth, Bounds.OuterHeight);
        context.FillPreserve();

        context.SetSourceRGBA(_clear);
        context.Stroke();
    }
}
