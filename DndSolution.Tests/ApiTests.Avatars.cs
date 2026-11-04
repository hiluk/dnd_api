using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using DndSolution.Application.Services;
using SkiaSharp;

namespace DndSolution.Tests;

/// <summary>
/// Своя картинка персонажа
/// </summary>
public partial class ApiTests
{
    private const int FrameMode = 1, FigureMode = 2;

    private async Task<(HttpClient Client, string Id)> CharacterAsync()
    {
        var (client, _) = await LoggedClientAsync();
        var created = await client.PostAsJsonAsync("/characters/create", NewCharacter("Арагорн"));
        var id = (await created.Content.ReadFromJsonAsync<JsonObject>())!["id"]!.GetValue<string>();
        return (client, id);
    }

    private static Task<HttpResponseMessage> UploadAsync(HttpClient client, string id, byte[] bytes, string type)
    {
        var file = new ByteArrayContent(bytes);
        file.Headers.ContentType = new MediaTypeHeaderValue(type);
        var form = new MultipartFormDataContent { { file, "file", "avatar" } };
        return client.PutAsync($"/characters/{id}/avatar", form);
    }

    /// <summary>
    /// Картинка: каждая точка закрашена функцией цвета
    /// </summary>
    private static byte[] Image(int width, int height, Func<int, int, SKColor> color,
        SKEncodedImageFormat format)
    {
        using var bitmap = new SKBitmap(new SKImageInfo(width, height, SKColorType.Rgba8888, SKAlphaType.Unpremul));
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                bitmap.SetPixel(x, y, color(x, y));
            }
        }

        using var image = SKImage.FromBitmap(bitmap);
        return image.Encode(format, 90).ToArray();
    }

    /// <summary>
    /// Фигура: круг посередине на прозрачном фоне
    /// </summary>
    private static byte[] FigurePng(int size = 120) => Image(size, size, (x, y) =>
    {
        var dx = x - size / 2;
        var dy = y - size / 2;
        return dx * dx + dy * dy < size * size / 9 ? SKColors.Red : SKColors.Transparent;
    }, SKEncodedImageFormat.Png);

    private static byte[] PhotoJpeg(int width = 160, int height = 120) =>
        Image(width, height, (x, _) => x < width / 2 ? SKColors.Red : SKColors.Blue, SKEncodedImageFormat.Jpeg);

    private static SKBitmap Decode(byte[] bytes) => SKBitmap.Decode(bytes);

    private string[] AvatarFiles() => Directory.Exists(_factory.AvatarsPath)
        ? Directory.GetFiles(_factory.AvatarsPath)
        : [];

    [Fact]
    public async Task Картинка_с_прозрачным_фоном_загружается_и_отдаётся_владельцу()
    {
        var (client, id) = await CharacterAsync();

        var response = await UploadAsync(client, id, FigurePng(), "image/png");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var avatar = (await response.Content.ReadFromJsonAsync<JsonObject>())!;
        Assert.True(avatar["hasTransparency"]!.GetValue<bool>());
        Assert.Equal(FrameMode, avatar["mode"]!.GetValue<int>());
        Assert.Equal(1, avatar["scale"]!.GetValue<double>());
        var url = avatar["url"]!.GetValue<string>();
        Assert.StartsWith($"/characters/{id}/avatar?v=", url);

        // Картинка видна в персонаже
        var character = await client.GetFromJsonAsync<JsonObject>($"/characters/{id}");
        Assert.Equal(url, character!["avatar"]!["url"]!.GetValue<string>());

        var file = await client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, file.StatusCode);
        Assert.Equal("image/png", file.Content.Headers.ContentType!.MediaType);
        Assert.Contains("private", file.Headers.CacheControl!.ToString());
        using var bitmap = Decode(await file.Content.ReadAsByteArrayAsync());
        Assert.Equal(0, bitmap.GetPixel(0, 0).Alpha);
    }

    [Fact]
    public async Task Фото_без_прозрачности_нельзя_показать_фигурой()
    {
        var (client, id) = await CharacterAsync();

        var avatar = await (await UploadAsync(client, id, PhotoJpeg(), "image/jpeg"))
            .Content.ReadFromJsonAsync<JsonObject>();
        Assert.False(avatar!["hasTransparency"]!.GetValue<bool>());

        var figure = await client.PatchAsJsonAsync($"/characters/{id}/avatar",
            new { mode = FigureMode, offsetX = 0, offsetY = 0, scale = 1 });
        Assert.Equal(HttpStatusCode.BadRequest, figure.StatusCode);
        var problem = (await figure.Content.ReadFromJsonAsync<JsonObject>())!;
        Assert.Equal("Картинка не подходит", problem["title"]!.GetValue<string>());
        Assert.NotNull(problem["errors"]!["Mode"]);

        // Кадр в рамке сохраняется
        var frame = await client.PatchAsJsonAsync($"/characters/{id}/avatar",
            new { mode = FrameMode, offsetX = -0.5, offsetY = 0.25, scale = 2 });
        Assert.Equal(HttpStatusCode.OK, frame.StatusCode);
        var saved = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["avatar"]!;
        Assert.Equal(-0.5, saved["offsetX"]!.GetValue<double>());
        Assert.Equal(0.25, saved["offsetY"]!.GetValue<double>());
        Assert.Equal(2, saved["scale"]!.GetValue<double>());
    }

    [Fact]
    public async Task Прозрачную_картинку_можно_показать_фигурой()
    {
        var (client, id) = await CharacterAsync();
        await UploadAsync(client, id, FigurePng(), "image/png");

        var figure = await client.PatchAsJsonAsync($"/characters/{id}/avatar",
            new { mode = FigureMode, offsetX = 0, offsetY = 0, scale = 1 });

        Assert.Equal(HttpStatusCode.OK, figure.StatusCode);
        Assert.Equal(FigureMode,
            (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["avatar"]!["mode"]!.GetValue<int>());
    }

    [Fact]
    public async Task Неверный_кадр_отклоняется()
    {
        var (client, id) = await CharacterAsync();
        await UploadAsync(client, id, PhotoJpeg(), "image/jpeg");

        var response = await client.PatchAsJsonAsync($"/characters/{id}/avatar",
            new { mode = FrameMode, offsetX = 3, offsetY = 0, scale = 10 });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var errors = (await response.Content.ReadFromJsonAsync<JsonObject>())!["errors"]!;
        Assert.NotNull(errors["OffsetX"]);
        Assert.NotNull(errors["Scale"]);
    }

    [Fact]
    public async Task Большая_картинка_уменьшается()
    {
        var (client, id) = await CharacterAsync();
        await UploadAsync(client, id, PhotoJpeg(2000, 1000), "image/jpeg");

        var url = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["avatar"]!["url"]!.GetValue<string>();
        using var bitmap = Decode(await client.GetByteArrayAsync(url));

        Assert.Equal(1024, bitmap.Width);
        Assert.Equal(512, bitmap.Height);
    }

    [Fact]
    public async Task Фото_с_телефона_поворачивается_по_EXIF()
    {
        var (client, id) = await CharacterAsync();
        // Левая половина красная, правая синяя; EXIF «повернуть на 90° по часовой»
        var jpeg = WithExifOrientation(PhotoJpeg(160, 120), 6);

        await UploadAsync(client, id, jpeg, "image/jpeg");
        var url = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["avatar"]!["url"]!.GetValue<string>();
        using var bitmap = Decode(await client.GetByteArrayAsync(url));

        // После поворота картинка вертикальная, красная половина сверху
        Assert.Equal(120, bitmap.Width);
        Assert.Equal(160, bitmap.Height);
        Assert.True(bitmap.GetPixel(60, 20).Red > 200);
        Assert.True(bitmap.GetPixel(60, 140).Blue > 200);
    }

    [Fact]
    public async Task Не_картинка_и_неподходящий_формат_отклоняются()
    {
        var (client, id) = await CharacterAsync();

        var text = await UploadAsync(client, id, "не картинка"u8.ToArray(), "image/png");
        Assert.Equal(HttpStatusCode.BadRequest, text.StatusCode);

        // GIF 1×1
        var gif = Convert.FromBase64String("R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==");
        var gifResponse = await UploadAsync(client, id, gif, "image/gif");
        Assert.Equal(HttpStatusCode.BadRequest, gifResponse.StatusCode);
        var errors = (await gifResponse.Content.ReadFromJsonAsync<JsonObject>())!["errors"]!;
        Assert.Contains("JPEG, PNG и WebP", errors["File"]![0]!.GetValue<string>());

        Assert.Null((await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["avatar"]);
    }

    [Fact]
    public async Task Файл_больше_5_МБ_отклоняется()
    {
        var (client, id) = await CharacterAsync();

        var response = await UploadAsync(client, id, new byte[6 * 1024 * 1024], "image/png");

        Assert.True(response.StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.RequestEntityTooLarge,
            $"Код ответа: {response.StatusCode}");
    }

    [Fact]
    public async Task Чужую_картинку_не_загрузить_и_не_получить()
    {
        var (owner, id) = await CharacterAsync();
        var (stranger, _) = await LoggedClientAsync();
        await UploadAsync(owner, id, PhotoJpeg(), "image/jpeg");

        Assert.Equal(HttpStatusCode.NotFound, (await UploadAsync(stranger, id, PhotoJpeg(), "image/jpeg")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await stranger.GetAsync($"/characters/{id}/avatar")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await stranger.DeleteAsync($"/characters/{id}/avatar")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await owner.GetAsync($"/characters/{id}/avatar")).StatusCode);
    }

    [Fact]
    public async Task Замена_и_удаление_не_оставляют_лишних_файлов()
    {
        var (client, id) = await CharacterAsync();

        var first = await (await UploadAsync(client, id, PhotoJpeg(), "image/jpeg")).Content.ReadFromJsonAsync<JsonObject>();
        var firstFile = first!["url"]!.GetValue<string>().Split("v=")[1];
        Assert.Contains(AvatarFiles(), f => f.Contains(firstFile));

        // Новая загрузка удаляет прежний файл
        var second = await (await UploadAsync(client, id, FigurePng(), "image/png")).Content.ReadFromJsonAsync<JsonObject>();
        var secondFile = second!["url"]!.GetValue<string>().Split("v=")[1];
        Assert.DoesNotContain(AvatarFiles(), f => f.Contains(firstFile));
        Assert.Contains(AvatarFiles(), f => f.Contains(secondFile));

        // Возврат к стандартной картинке удаляет файл
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/characters/{id}/avatar")).StatusCode);
        Assert.DoesNotContain(AvatarFiles(), f => f.Contains(secondFile));
        Assert.Null((await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["avatar"]);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/characters/{id}/avatar")).StatusCode);

        // Удаление персонажа удаляет и его картинку
        var third = await (await UploadAsync(client, id, PhotoJpeg(), "image/jpeg")).Content.ReadFromJsonAsync<JsonObject>();
        var thirdFile = third!["url"]!.GetValue<string>().Split("v=")[1];
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/characters/{id}")).StatusCode);
        Assert.DoesNotContain(AvatarFiles(), f => f.Contains(thirdFile));
    }

    /// <summary>
    /// Все восемь ориентаций EXIF. Картинка 160×120 из четырёх цветных четвертей;
    /// проверяем, в каком углу оказались красная (была слева сверху)
    /// и зелёная (была справа сверху) четверти.
    /// </summary>
    [Theory]
    [InlineData(1, "TL", "TR")]
    [InlineData(2, "TR", "TL")]
    [InlineData(3, "BR", "BL")]
    [InlineData(4, "BL", "BR")]
    [InlineData(5, "TL", "BL")]
    [InlineData(6, "TR", "BR")]
    [InlineData(7, "BR", "TR")]
    [InlineData(8, "BL", "TL")]
    public void Все_ориентации_EXIF(ushort orientation, string red, string green)
    {
        const int width = 160, height = 120;
        var jpeg = Image(width, height, (x, y) => (x < width / 2, y < height / 2) switch
        {
            (true, true) => SKColors.Red,
            (false, true) => SKColors.Lime,
            (true, false) => SKColors.Blue,
            _ => SKColors.White,
        }, SKEncodedImageFormat.Jpeg);

        var processed = new SkiaImageProcessor().Process(WithExifOrientation(jpeg, orientation));
        using var bitmap = Decode(processed.Content);

        var swapped = orientation >= 5;
        Assert.Equal(swapped ? height : width, bitmap.Width);
        Assert.Equal(swapped ? width : height, bitmap.Height);

        SKColor Corner(string corner) => bitmap.GetPixel(
            corner[1] == 'L' ? 10 : bitmap.Width - 10,
            corner[0] == 'T' ? 10 : bitmap.Height - 10);

        var redPixel = Corner(red);
        var greenPixel = Corner(green);
        Assert.True(redPixel.Red > 200 && redPixel.Green < 60, $"красный в {red}: {redPixel}");
        Assert.True(greenPixel.Green > 200 && greenPixel.Red < 60, $"зелёный в {green}: {greenPixel}");
    }

    /// <summary>
    /// Вставить в JPEG блок EXIF с одним тегом Orientation
    /// </summary>
    private static byte[] WithExifOrientation(byte[] jpeg, ushort orientation)
    {
        byte[] tiff =
        [
            (byte)'M', (byte)'M', 0, 42, 0, 0, 0, 8, // заголовок TIFF, IFD с 8-го байта
            0, 1, // одна запись
            0x01, 0x12, 0, 3, 0, 0, 0, 1, (byte)(orientation >> 8), (byte)orientation, 0, 0, // Orientation, SHORT
            0, 0, 0, 0, // следующего IFD нет
        ];
        byte[] header = [(byte)'E', (byte)'x', (byte)'i', (byte)'f', 0, 0];
        var length = 2 + header.Length + tiff.Length;
        byte[] app1 = [0xFF, 0xE1, (byte)(length >> 8), (byte)length, .. header, .. tiff];

        // Сразу после маркера начала файла FFD8
        return [.. jpeg[..2], .. app1, .. jpeg[2..]];
    }
}
