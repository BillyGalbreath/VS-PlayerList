using Cairo;
using SkiaSharp;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui;

public sealed class HeaderImage(ICoreClientAPI api, string url, ElementBounds bounds) : GuiElement(api, bounds) {
    private readonly BitmapRef? _bitmap = LoadBitmapAsync(url).Result;

    private static async Task<BitmapRef?> LoadBitmapAsync(string url) {
        using HttpClient client = new();
        try {
            byte[] bytes = await client.GetByteArrayAsync(url);
            using SKData data = SKData.CreateCopy(bytes);
            SKBitmap skBitmap = SKBitmap.Decode(SKData.CreateCopy(bytes));
            return new BitmapExternal(skBitmap);
        } catch (HttpRequestException ex) {
            throw new Exception($"Error downloading image from URL: {ex.Message}");
        } catch (Exception ex) {
            throw new Exception($"Error decoding image: {ex.Message}");
        }
    }

    public override void ComposeElements(Context ctx, ImageSurface surface) {
        if (_bitmap == null) {
            return;
        }

        Bounds.CalcWorldBounds();

        ctx.SetSourceRGBA(1.0, 1.0, 1.0, 1.0);
        surface.Image(_bitmap, (int)Bounds.drawX, (int)Bounds.drawY, (int)Bounds.fixedWidth, (int)Bounds.fixedHeight);
    }

    public HeaderImage SetBounds(ElementBounds bounds) {
        Bounds = bounds;

        if (_bitmap == null) {
            return this;
        }

        if (Bounds.fixedWidth == 0) {
            Bounds.fixedWidth = _bitmap.Width;
        }

        if (Bounds.fixedHeight == 0) {
            Bounds.fixedHeight = _bitmap.Height;
        }

        return this;
    }
}
