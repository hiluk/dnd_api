using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Rules;

/// <summary>
/// Значения шести характеристик
/// </summary>
public sealed record AbilityScores(int Strength, int Dexterity, int Constitution, int Intelligence, int Wisdom, int Charisma)
{
    public static readonly IReadOnlyList<Ability> All =
        [Ability.Strength, Ability.Dexterity, Ability.Constitution, Ability.Intelligence, Ability.Wisdom, Ability.Charisma];

    public int this[Ability ability] => ability switch
    {
        Ability.Strength => Strength,
        Ability.Dexterity => Dexterity,
        Ability.Constitution => Constitution,
        Ability.Intelligence => Intelligence,
        Ability.Wisdom => Wisdom,
        Ability.Charisma => Charisma,
        _ => throw new ArgumentOutOfRangeException(nameof(ability), ability, null),
    };

    public IEnumerable<int> Values => All.Select(a => this[a]);

    public static AbilityScores From(Func<Ability, int> value) => new(
        value(Ability.Strength), value(Ability.Dexterity), value(Ability.Constitution),
        value(Ability.Intelligence), value(Ability.Wisdom), value(Ability.Charisma));

    public static AbilityScores From(CharacterStats stats) => new(
        stats.Strength, stats.Dexterity, stats.Constitution, stats.Intelligence, stats.Wisdom, stats.Charisma);

    public CharacterStats ToStats() => new()
    {
        Strength = Strength,
        Dexterity = Dexterity,
        Constitution = Constitution,
        Intelligence = Intelligence,
        Wisdom = Wisdom,
        Charisma = Charisma,
    };
}
