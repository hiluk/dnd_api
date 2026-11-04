using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Avatars;

/// <summary>
/// Своя картинка персонажа, загруженная пользователем
/// </summary>
public sealed record CharacterAvatar
{
    /// <summary>
    /// Имя файла в хранилище. Новое при каждой загрузке — служит и версией для кэша
    /// </summary>
    public required string FileName { get; init; }

    /// <summary>
    /// Фон по краям прозрачный: картинку можно показать фигурой
    /// </summary>
    public bool HasTransparency { get; init; }

    public AvatarMode Mode { get; init; } = AvatarMode.Frame;

    /// <summary>
    /// Сдвиг кадра по горизонтали, от -1 до 1 (как Alignment во Flutter)
    /// </summary>
    public double OffsetX { get; init; }

    /// <summary>
    /// Сдвиг кадра по вертикали, от -1 до 1
    /// </summary>
    public double OffsetY { get; init; }

    /// <summary>
    /// Увеличение, от 1 до 4
    /// </summary>
    public double Scale { get; init; } = 1;
}

/// <summary>
/// Вид и кадр картинки, которые меняет пользователь
/// </summary>
public sealed record AvatarLayout(AvatarMode Mode, double OffsetX, double OffsetY, double Scale);

/// <summary>
/// Картинка после обработки: уменьшена, повёрнута по EXIF и перекодирована
/// </summary>
/// <param name="Content">Байты файла</param>
/// <param name="Extension">Расширение без точки: png или jpg</param>
/// <param name="HasTransparency">Фон по краям прозрачный</param>
public sealed record ProcessedImage(byte[] Content, string Extension, bool HasTransparency);

/// <summary>
/// Файл картинки для отдачи клиенту
/// </summary>
public sealed record AvatarFile(Stream Content, string ContentType);

/// <summary>
/// Картинку нельзя принять или настройки вида неверны
/// </summary>
/// <param name="errors">Ошибки по полям</param>
public sealed class AvatarException(IReadOnlyDictionary<string, string[]> errors)
    : Exception(string.Join("; ", errors.SelectMany(e => e.Value)))
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;

    public AvatarException(string field, string message)
        : this(new Dictionary<string, string[]> { [field] = [message] })
    {
    }
}
