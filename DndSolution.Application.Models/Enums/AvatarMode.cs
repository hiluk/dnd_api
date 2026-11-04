namespace DndSolution.Application.Models.Enums;

/// <summary>
/// Как показывать свою картинку персонажа
/// </summary>
public enum AvatarMode
{
    /// <summary>
    /// Внутри рамки карточки, с кадрированием
    /// </summary>
    Frame = 1,

    /// <summary>
    /// Фигура выходит за верх карточки. Только для картинок с прозрачным фоном
    /// </summary>
    Figure = 2,
}
