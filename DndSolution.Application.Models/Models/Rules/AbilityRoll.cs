namespace DndSolution.Application.Models.Models.Rules;

/// <summary>
/// Результат бросков 4к6 и подпись, подтверждающая, что значения выпали на сервере
/// </summary>
public sealed record AbilityRoll(IReadOnlyList<int> Values, string Token);
