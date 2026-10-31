using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Rules;

/// <summary>
/// Исходные данные персонажа, по которым считается лист
/// </summary>
/// <param name="RolledValues">Подтверждённые сервером результаты бросков (только для <see cref="GenerationMethod.Roll"/>)</param>
public sealed record CharacterDraft(
    CharacterRaceType Race,
    CharacterClassType Class,
    GenerationMethod Method,
    AbilityScores BaseScores,
    IReadOnlyList<Ability> BonusChoices,
    IReadOnlyList<int>? RolledValues = null);
