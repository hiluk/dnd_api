using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Avatars;
using SkiaSharp;

namespace DndSolution.Application.Services;

/// <summary>
/// Проверяет и готовит картинку персонажа: формат, размер, поворот по EXIF,
/// уменьшение и признак прозрачного фона.
/// </summary>
public sealed class SkiaImageProcessor : IImageProcessor
{
    /// <summary>
    /// Наибольший размер загружаемого файла
    /// </summary>
    public const int MaxFileBytes = 5 * 1024 * 1024;

    /// <summary>
    /// Длинная сторона после уменьшения
    /// </summary>
    public const int MaxSide = 1024;

    /// <summary>
    /// Защита от «бомб»: маленький файл с огромными размерами
    /// </summary>
    private const int MaxSourceSide = 10_000;

    private const string Field = "File";

    public ProcessedImage Process(byte[] content)
    {
        if (content.Length == 0)
        {
            throw new AvatarException(Field, "Файл пустой");
        }

        if (content.Length > MaxFileBytes)
        {
            throw new AvatarException(Field, "Файл больше 5 МБ");
        }

        using var data = SKData.CreateCopy(content);
        using var codec = SKCodec.Create(data)
            ?? throw new AvatarException(Field, "Это не картинка или файл повреждён");

        if (codec.EncodedFormat is not (SKEncodedImageFormat.Jpeg or SKEncodedImageFormat.Png
            or SKEncodedImageFormat.Webp))
        {
            throw new AvatarException(Field, "Подходят только JPEG, PNG и WebP");
        }

        if (codec.Info.Width > MaxSourceSide || codec.Info.Height > MaxSourceSide)
        {
            throw new AvatarException(Field, $"Картинка больше {MaxSourceSide} точек по стороне");
        }

        var info = codec.Info.WithColorType(SKColorType.Rgba8888).WithAlphaType(SKAlphaType.Unpremul);
        using var decoded = new SKBitmap(info);
        var result = codec.GetPixels(info, decoded.GetPixels());
        if (result is not (SKCodecResult.Success or SKCodecResult.IncompleteInput))
        {
            throw new AvatarException(Field, "Не удалось прочитать картинку");
        }

        using var oriented = ApplyOrigin(decoded, codec.EncodedOrigin);
        using var resized = Resize(oriented);
        var hasTransparency = HasTransparentBorder(resized);
        var hasAlpha = hasTransparency || HasAnyAlpha(resized);

        using var image = SKImage.FromBitmap(resized);
        using var encoded = hasAlpha
            ? image.Encode(SKEncodedImageFormat.Png, 100)
            : image.Encode(SKEncodedImageFormat.Jpeg, 88);

        return new ProcessedImage(encoded.ToArray(), hasAlpha ? "png" : "jpg", hasTransparency);
    }

    /// <summary>
    /// Фон прозрачный, если больше половины точек по краям почти прозрачны.
    /// То же правило проверяет тест картинок в приложении.
    /// </summary>
    public static bool HasTransparentBorder(SKBitmap bitmap)
    {
        int clear = 0, total = 0;

        void Check(int x, int y)
        {
            total++;
            if (bitmap.GetPixel(x, y).Alpha < 16)
            {
                clear++;
            }
        }

        for (var x = 0; x < bitmap.Width; x++)
        {
            Check(x, 0);
            Check(x, bitmap.Height - 1);
        }

        for (var y = 0; y < bitmap.Height; y++)
        {
            Check(0, y);
            Check(bitmap.Width - 1, y);
        }

        return total > 0 && clear * 2 > total;
    }

    private static bool HasAnyAlpha(SKBitmap bitmap)
    {
        foreach (var pixel in bitmap.Pixels)
        {
            if (pixel.Alpha < 255)
            {
                return true;
            }
        }

        return false;
    }

    private static SKBitmap Resize(SKBitmap source)
    {
        var longest = Math.Max(source.Width, source.Height);
        if (longest <= MaxSide)
        {
            return source.Copy();
        }

        var ratio = (double)MaxSide / longest;
        var info = new SKImageInfo(
            Math.Max(1, (int)Math.Round(source.Width * ratio)),
            Math.Max(1, (int)Math.Round(source.Height * ratio)),
            SKColorType.Rgba8888,
            SKAlphaType.Unpremul);

        return source.Resize(info, SKFilterQuality.High);
    }

    /// <summary>
    /// Фото с телефона хранят поворот в EXIF: поворачиваем сами,
    /// потому что после перекодирования EXIF теряется
    /// </summary>
    private static SKBitmap ApplyOrigin(SKBitmap source, SKEncodedOrigin origin)
    {
        if (origin is SKEncodedOrigin.TopLeft or SKEncodedOrigin.Default)
        {
            return source.Copy();
        }

        var swap = origin is SKEncodedOrigin.LeftTop or SKEncodedOrigin.RightTop
            or SKEncodedOrigin.RightBottom or SKEncodedOrigin.LeftBottom;
        var width = swap ? source.Height : source.Width;
        var height = swap ? source.Width : source.Height;

        var result = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul));
        using var canvas = new SKCanvas(result);
        canvas.Clear(SKColors.Transparent);

        switch (origin)
        {
            case SKEncodedOrigin.TopRight:
                canvas.Scale(-1, 1, width / 2f, 0);
                break;
            case SKEncodedOrigin.BottomRight:
                canvas.RotateDegrees(180, width / 2f, height / 2f);
                break;
            case SKEncodedOrigin.BottomLeft:
                canvas.Scale(1, -1, 0, height / 2f);
                break;
            case SKEncodedOrigin.LeftTop:
                canvas.Translate(width, 0);
                canvas.RotateDegrees(90);
                canvas.Scale(1, -1, 0, source.Height / 2f);
                break;
            case SKEncodedOrigin.RightTop:
                canvas.Translate(width, 0);
                canvas.RotateDegrees(90);
                break;
            case SKEncodedOrigin.RightBottom:
                canvas.Translate(width, 0);
                canvas.RotateDegrees(90);
                canvas.Scale(-1, 1, source.Width / 2f, 0);
                break;
            case SKEncodedOrigin.LeftBottom:
                canvas.Translate(0, height);
                canvas.RotateDegrees(-90);
                break;
        }

        canvas.DrawBitmap(source, 0, 0);
        canvas.Flush();

        return result;
    }
}
