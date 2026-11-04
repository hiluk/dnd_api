using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Своя картинка персонажа
/// </summary>
public class CharacterAvatarDto
{
    /// <summary>
    /// Адрес файла относительно API. Меняется при каждой загрузке
    /// </summary>
    public required string Url { get; init; }

    /// <summary>
    /// Фон по краям прозрачный: можно показать фигурой
    /// </summary>
    public bool HasTransparency { get; init; }

    public AvatarModeDto Mode { get; init; }

    /// <summary>
    /// Сдвиг кадра по горизонтали, от -1 до 1
    /// </summary>
    public double OffsetX { get; init; }

    /// <summary>
    /// Сдвиг кадра по вертикали, от -1 до 1
    /// </summary>
    public double OffsetY { get; init; }

    /// <summary>
    /// Увеличение, от 1 до 4
    /// </summary>
    public double Scale { get; init; }
}

/// <summary>
/// Новый вид и кадр картинки
/// </summary>
public class UpdateAvatarRequest
{
    public AvatarModeDto Mode { get; init; } = AvatarModeDto.Frame;

    public double OffsetX { get; init; }

    public double OffsetY { get; init; }

    public double Scale { get; init; } = 1;
}
