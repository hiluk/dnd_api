using DndSolution.Application.Models.Models.Rules;

namespace DndSolution.Application.Abstractions;

/// <summary>
/// Броски характеристик на сервере
/// </summary>
public interface IAbilityRollService
{
    /// <summary>
    /// Бросить 4к6 без наименьшего шесть раз
    /// </summary>
    /// <param name="userId">Пользователь, которому выдаётся результат</param>
    public AbilityRoll Roll(Guid userId);

    /// <summary>
    /// Проверить подпись результата бросков
    /// </summary>
    /// <returns>Значения бросков или null, если подпись неверна, просрочена или выдана другому пользователю</returns>
    public IReadOnlyList<int>? Verify(string? token, Guid userId);
}
