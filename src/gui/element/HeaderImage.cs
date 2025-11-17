using Cairo;
using SkiaSharp;
using Vintagestory.API.Client;
using Vintagestory.API.Common;

namespace playerlist.gui.element;

public sealed class HeaderImage : GuiElement {
    private readonly PlayerList _mod;
    private readonly BitmapRef? _bitmap;
    private readonly BitmapRef _missing;

    public HeaderImage(PlayerList mod, string url, ElementBounds bounds) : base(mod.Api as ICoreClientAPI, bounds) {
        _mod = mod;
        _bitmap = url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase) ? LoadBitmapAsync(url).Result : LoadBitmapAsset(url);
        _missing = LoadBitmapAsset(new AssetLocation("playerlist", "textures/missing.png"))!;
    }

    private BitmapRef? LoadBitmapAsset(string path) {
        return api.Assets.Get(new AssetLocation(path)).ToBitmap(api);
    }

    private BitmapRef? LoadBitmapAsset(AssetLocation asset) {
        return api.Assets.Get(asset).ToBitmap(api);
    }

    private async Task<BitmapRef?> LoadBitmapAsync(string url) {
        using HttpClient client = new();
        try {
            byte[] bytes = await client.GetByteArrayAsync(url);
            using SKData data = SKData.CreateCopy(bytes);
            SKBitmap skBitmap = SKBitmap.Decode(SKData.CreateCopy(bytes));
            return new BitmapExternal(skBitmap);
        } catch (HttpRequestException ex) {
            _mod.Logger.Error($"Error downloading image from URL: {ex.Message}");
        } catch (Exception ex) {
            _mod.Logger.Error($"Error decoding image: {ex.Message}");
        }

        return null;
    }

    public override void ComposeElements(Context ctx, ImageSurface surface) {
        Bounds.CalcWorldBounds();

        ctx.SetSourceRGBA(1.0, 1.0, 1.0, 1.0);

        try {
            Draw(surface, _bitmap ?? _missing);
        } catch (Exception) {
            Draw(surface, _missing);
        }
    }

    private void Draw(ImageSurface surface, BitmapRef bitmap) {
        surface.Image(bitmap, (int)Bounds.drawX, (int)Bounds.drawY, (int)scaled(Bounds.fixedWidth), (int)scaled(Bounds.fixedHeight));
    }

    public HeaderImage SetBounds(ElementBounds bounds) {
        Bounds = bounds;

        try {
            if (Bounds.fixedWidth == 0) {
                Bounds.fixedWidth = _bitmap?.Width ?? _missing.Width;
            }

            if (Bounds.fixedHeight == 0) {
                Bounds.fixedHeight = _bitmap?.Height ?? _missing.Height;
            }
        } catch (Exception) {
            Bounds.fixedWidth = _missing.Width;
            Bounds.fixedHeight = _missing.Height;
        }

        return this;
    }
}
