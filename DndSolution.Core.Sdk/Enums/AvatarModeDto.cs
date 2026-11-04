namespace Core.Sdk.Enums;

/// <summary>
/// Как показывать свою картинку персонажа
/// </summary>
public enum AvatarModeDto
{
    /// <summary>
    /// Внутри рамки карточки
    /// </summary>
    Frame = 1,

    /// <summary>
    /// Фигура выходит за верх карточки (только при прозрачном фоне)
    /// </summary>
    Figure = 2,
}
