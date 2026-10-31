namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Результат бросков 4к6 для шести характеристик
/// </summary>
public class AbilityRollDto
{
    /// <summary>
    /// Шесть значений в порядке выпадения
    /// </summary>
    public required List<int> Values { get; init; }

    /// <summary>
    /// Подпись результата, передаётся в черновике как <c>rollToken</c>. Действует час.
    /// </summary>
    public required string Token { get; init; }
}
