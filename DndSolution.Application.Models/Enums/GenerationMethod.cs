namespace DndSolution.Application.Models.Enums;

/// <summary>
/// Способ генерации характеристик
/// </summary>
public enum GenerationMethod
{
    /// <summary>
    /// Стандартный набор 15, 14, 13, 12, 10, 8
    /// </summary>
    StandardArray = 1,

    /// <summary>
    /// Покупка очков (27 очков, значения 8–15)
    /// </summary>
    PointBuy = 2,

    /// <summary>
    /// Броски 4к6 без наименьшего
    /// </summary>
    Roll = 3,
}
