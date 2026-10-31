using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Rules;

/// <summary>
/// Вычисленные значения персонажа
/// </summary>
public sealed record CharacterSheet(
    AbilityScores Scores,
    AbilityScores Modifiers,
    int HitPoints,
    int ArmorClass,
    int Initiative,
    int Speed,
    int ProficiencyBonus,
    IReadOnlyList<SavingThrow> SavingThrows);

/// <summary>
/// Спасбросок: итоговый бонус и владение
/// </summary>
public sealed record SavingThrow(Ability Ability, int Bonus, bool Proficient);
