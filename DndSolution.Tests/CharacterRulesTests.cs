using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using DndSolution.Application.Models.Models.Rules;
using DndSolution.Application.Services.Rules;

namespace DndSolution.Tests;

public class CharacterRulesTests
{
    private static readonly AbilityScores StandardBase = new(15, 14, 13, 12, 10, 8);

    private static Race Human => new()
    {
        Name = "Человек",
        Type = CharacterRaceType.Human,
        Asi = AbilityScores.All.Select(a => new Asi { Stat = "", Value = 1, Ability = a }).ToList(),
        Speed = [new Speed { Type = "Ходьба", Value = 30 }],
    };

    private static Race HalfElf => new()
    {
        Name = "Полуэльф",
        Type = CharacterRaceType.HalfElf,
        Asi =
        [
            new Asi { Stat = "Харизма", Value = 2, Ability = Ability.Charisma },
            new Asi { Stat = "Две другие на выбор", Value = 1, Choices = 2 },
        ],
        Speed = [new Speed { Type = "Ходьба", Value = 30 }],
    };

    private static Race Dwarf => new()
    {
        Name = "Дварф",
        Type = CharacterRaceType.Dwarf,
        Asi = [new Asi { Stat = "Телосложение", Value = 2, Ability = Ability.Constitution }],
        Speed = [new Speed { Type = "Ходьба", Value = 25 }],
    };

    private static CharacterClass Class(CharacterClassType type, int hitDie, params Ability[] saves) =>
        new() { Name = type.ToString(), Type = type, HitDie = hitDie, SavingThrows = saves.ToList() };

    private static CharacterClass Fighter => Class(CharacterClassType.Fighter, 10, Ability.Strength, Ability.Constitution);

    private static CharacterDraft Draft(
        AbilityScores scores,
        GenerationMethod method = GenerationMethod.StandardArray,
        IReadOnlyList<Ability>? choices = null,
        IReadOnlyList<int>? rolled = null,
        CharacterRaceType race = CharacterRaceType.Dwarf) =>
        new(race, CharacterClassType.Fighter, method, scores, choices ?? [], rolled);

    private static CharacterRulesException Violation(CharacterDraft draft, Race race, CharacterClass? characterClass = null) =>
        Assert.Throws<CharacterRulesException>(() => CharacterRules.Evaluate(draft, race, characterClass ?? Fighter));

    [Theory]
    [InlineData(8, -1)]
    [InlineData(9, -1)]
    [InlineData(10, 0)]
    [InlineData(11, 0)]
    [InlineData(15, 2)]
    [InlineData(20, 5)]
    [InlineData(3, -4)]
    public void Модификатор_характеристики(int score, int modifier)
    {
        Assert.Equal(modifier, CharacterRules.Modifier(score));
    }

    [Fact]
    public void Стандартный_набор_в_любом_порядке_принимается()
    {
        var sheet = CharacterRules.Evaluate(Draft(new AbilityScores(8, 10, 12, 13, 14, 15)), Dwarf, Fighter);

        Assert.Equal(14, sheet.Scores.Constitution);
    }

    [Fact]
    public void Повтор_в_стандартном_наборе_отклоняется()
    {
        var error = Violation(Draft(new AbilityScores(15, 15, 13, 12, 10, 8)), Dwarf);

        Assert.Contains("15", error.Errors["BaseStats"].Single());
    }

    [Fact]
    public void Покупка_очков_в_пределах_бюджета()
    {
        // 15, 15, 15 = 27 очков
        var sheet = CharacterRules.Evaluate(
            Draft(new AbilityScores(15, 15, 15, 8, 8, 8), GenerationMethod.PointBuy), Dwarf, Fighter);

        Assert.Equal(17, sheet.Scores.Constitution);
    }

    [Fact]
    public void Перерасход_очков_отклоняется()
    {
        var error = Violation(Draft(new AbilityScores(15, 15, 15, 9, 8, 8), GenerationMethod.PointBuy), Dwarf);

        Assert.Equal("Потрачено 28 очков из 27", error.Errors["BaseStats"].Single());
    }

    [Theory]
    [InlineData(7)]
    [InlineData(16)]
    public void Значения_вне_8_15_при_покупке_отклоняются(int value)
    {
        Violation(Draft(new AbilityScores(value, 8, 8, 8, 8, 8), GenerationMethod.PointBuy), Dwarf);
    }

    [Fact]
    public void Броски_это_перестановка_подтверждённых_значений()
    {
        int[] rolled = [17, 6, 12, 9, 14, 11];

        var sheet = CharacterRules.Evaluate(
            Draft(new AbilityScores(6, 9, 11, 12, 14, 17), GenerationMethod.Roll, rolled: rolled), Dwarf, Fighter);

        Assert.Equal(13, sheet.Scores.Constitution);
    }

    [Fact]
    public void Подмена_бросков_отклоняется()
    {
        Violation(Draft(new AbilityScores(18, 9, 11, 12, 14, 17), GenerationMethod.Roll, rolled: [17, 6, 12, 9, 14, 11]), Dwarf);
    }

    [Fact]
    public void Броски_без_подтверждения_отклоняются()
    {
        var error = Violation(Draft(StandardBase, GenerationMethod.Roll), Dwarf);

        Assert.Contains("бросьте заново", error.Errors["BaseStats"].Single());
    }

    [Fact]
    public void Человек_получает_плюс_один_ко_всему()
    {
        var sheet = CharacterRules.Evaluate(Draft(StandardBase, race: CharacterRaceType.Human), Human, Fighter);

        Assert.Equal(new AbilityScores(16, 15, 14, 13, 11, 9), sheet.Scores);
    }

    [Fact]
    public void Полуэльф_выбирает_две_характеристики()
    {
        var sheet = CharacterRules.Evaluate(
            Draft(StandardBase, choices: [Ability.Strength, Ability.Wisdom], race: CharacterRaceType.HalfElf),
            HalfElf, Fighter);

        Assert.Equal(new AbilityScores(16, 14, 13, 12, 11, 10), sheet.Scores);
    }

    [Theory]
    [InlineData(new[] { Ability.Strength })]
    [InlineData(new[] { Ability.Strength, Ability.Strength })]
    [InlineData(new[] { Ability.Strength, Ability.Charisma })]
    [InlineData(new[] { Ability.Strength, Ability.Wisdom, Ability.Dexterity })]
    public void Неверный_выбор_полуэльфа_отклоняется(Ability[] choices)
    {
        var error = Violation(Draft(StandardBase, choices: choices, race: CharacterRaceType.HalfElf), HalfElf);

        Assert.Equal("Полуэльф выбирает 2 разные характеристики, кроме: Харизма", error.Errors["BonusChoices"].Single());
    }

    [Fact]
    public void Выбор_у_расы_без_бонусов_на_выбор_отклоняется()
    {
        Violation(Draft(StandardBase, choices: [Ability.Strength]), Dwarf);
    }

    [Fact]
    public void Итоговое_значение_не_больше_20()
    {
        var sheet = CharacterRules.Evaluate(
            Draft(new AbilityScores(10, 10, 20, 10, 10, 10), GenerationMethod.Roll, rolled: [10, 10, 20, 10, 10, 10]),
            Dwarf, Fighter);

        Assert.Equal(20, sheet.Scores.Constitution);
    }

    [Fact]
    public void Хиты_волшебника_с_телосложением_8()
    {
        var wizard = Class(CharacterClassType.Wizard, 6, Ability.Intelligence, Ability.Wisdom);

        var sheet = CharacterRules.Evaluate(Draft(new AbilityScores(10, 14, 8, 15, 13, 12)), Human, wizard);

        // Человек: Телосложение 9 → модификатор -1
        Assert.Equal(5, sheet.HitPoints);
    }

    [Fact]
    public void Хиты_при_минимальном_телосложении()
    {
        var wizard = Class(CharacterClassType.Wizard, 6);

        var sheet = CharacterRules.Evaluate(
            Draft(new AbilityScores(10, 10, 3, 10, 10, 10), GenerationMethod.Roll, rolled: [10, 10, 3, 10, 10, 10]),
            Dwarf, wizard);

        // Дварф: Телосложение 3 + 2 = 5 → модификатор -3, 6 - 3 = 3
        Assert.Equal(3, sheet.HitPoints);
    }

    [Fact]
    public void Класс_доспеха_без_доспехов()
    {
        var scores = new AbilityScores(10, 14, 12, 15, 13, 8);
        var barbarian = Class(CharacterClassType.Barbarian, 12);
        var monk = Class(CharacterClassType.Monk, 8);

        // Дварф: Ловкость 14 (+2), Телосложение 14 (+2), Мудрость 13 (+1)
        Assert.Equal(12, CharacterRules.Evaluate(Draft(scores), Dwarf, Fighter).ArmorClass);
        Assert.Equal(14, CharacterRules.Evaluate(Draft(scores), Dwarf, barbarian).ArmorClass);
        Assert.Equal(13, CharacterRules.Evaluate(Draft(scores), Dwarf, monk).ArmorClass);
    }

    [Fact]
    public void Производные_значения_воина_дварфа()
    {
        var sheet = CharacterRules.Evaluate(Draft(StandardBase), Dwarf, Fighter);

        Assert.Equal(new AbilityScores(15, 14, 15, 12, 10, 8), sheet.Scores);
        Assert.Equal(new AbilityScores(2, 2, 2, 1, 0, -1), sheet.Modifiers);
        Assert.Equal(12, sheet.HitPoints);
        Assert.Equal(12, sheet.ArmorClass);
        Assert.Equal(2, sheet.Initiative);
        Assert.Equal(25, sheet.Speed);
        Assert.Equal(2, sheet.ProficiencyBonus);
        Assert.Equal(
            [(Ability.Strength, 4, true), (Ability.Dexterity, 2, false), (Ability.Constitution, 4, true),
             (Ability.Intelligence, 1, false), (Ability.Wisdom, 0, false), (Ability.Charisma, -1, false)],
            sheet.SavingThrows.Select(s => (s.Ability, s.Bonus, s.Proficient)));
    }
}
