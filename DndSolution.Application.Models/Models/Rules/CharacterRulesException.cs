namespace DndSolution.Application.Models.Models.Rules;

/// <summary>
/// Нарушение правил создания персонажа
/// </summary>
/// <param name="errors">Ошибки по полям черновика</param>
public sealed class CharacterRulesException(IReadOnlyDictionary<string, string[]> errors)
    : Exception(string.Join("; ", errors.SelectMany(e => e.Value)))
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
