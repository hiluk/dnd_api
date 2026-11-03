using DndSolution.Application.Models.Models.Character;

namespace DndSolution.Application.Models.Models.Rules;

/// <summary>
/// Персонаж вместе с вычисленным листом
/// </summary>
public sealed record CharacterWithSheet(Character.Character Character, CharacterSheet Sheet);
