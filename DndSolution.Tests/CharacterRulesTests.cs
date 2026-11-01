using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Backgrounds;
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

    /// <summary>
    /// Предыстория без навыков, чтобы проверять характеристики отдельно от навыков
    /// </summary>
    private static readonly Background NoBackground = new() { Name = "Без навыков", Type = BackgroundType.Custom };

    private static CharacterSheet Evaluate(CharacterDraft draft, Race race, CharacterClass characterClass) =>
        CharacterRules.Evaluate(draft, race, characterClass, NoBackground);

    private static CharacterRulesException Violation(CharacterDraft draft, Race race, CharacterClass? characterClass = null) =>
        Assert.Throws<CharacterRulesException>(() => Evaluate(draft, race, characterClass ?? Fighter));

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
        var sheet = Evaluate(Draft(new AbilityScores(8, 10, 12, 13, 14, 15)), Dwarf, Fighter);

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
        var sheet = Evaluate(
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

        var sheet = Evaluate(
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
        var sheet = Evaluate(Draft(StandardBase, race: CharacterRaceType.Human), Human, Fighter);

        Assert.Equal(new AbilityScores(16, 15, 14, 13, 11, 9), sheet.Scores);
    }

    [Fact]
    public void Полуэльф_выбирает_две_характеристики()
    {
        var sheet = Evaluate(
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
        var sheet = Evaluate(
            Draft(new AbilityScores(10, 10, 20, 10, 10, 10), GenerationMethod.Roll, rolled: [10, 10, 20, 10, 10, 10]),
            Dwarf, Fighter);

        Assert.Equal(20, sheet.Scores.Constitution);
    }

    [Fact]
    public void Хиты_волшебника_с_телосложением_8()
    {
        var wizard = Class(CharacterClassType.Wizard, 6, Ability.Intelligence, Ability.Wisdom);

        var sheet = Evaluate(Draft(new AbilityScores(10, 14, 8, 15, 13, 12)), Human, wizard);

        // Человек: Телосложение 9 → модификатор -1
        Assert.Equal(5, sheet.HitPoints);
    }

    [Fact]
    public void Хиты_при_минимальном_телосложении()
    {
        var wizard = Class(CharacterClassType.Wizard, 6);

        var sheet = Evaluate(
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
        Assert.Equal(12, Evaluate(Draft(scores), Dwarf, Fighter).ArmorClass);
        Assert.Equal(14, Evaluate(Draft(scores), Dwarf, barbarian).ArmorClass);
        Assert.Equal(13, Evaluate(Draft(scores), Dwarf, monk).ArmorClass);
    }

    [Fact]
    public void Производные_значения_воина_дварфа()
    {
        var sheet = Evaluate(Draft(StandardBase), Dwarf, Fighter);

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

    // ---- Навыки ----

    private static readonly Background Criminal = new()
    {
        Name = "Преступник",
        Type = BackgroundType.Criminal,
        Skills = [Skill.Deception, Skill.Stealth],
    };

    private static readonly Background Custom = new()
    {
        Name = "Своя предыстория",
        Type = BackgroundType.Custom,
        SkillChoices = 2,
    };

    private static CharacterClass Rogue => new()
    {
        Name = "Плут",
        Type = CharacterClassType.Rogue,
        HitDie = 8,
        SavingThrows = [Ability.Dexterity, Ability.Intelligence],
        SkillChoices = 4,
        SkillOptions =
        [
            Skill.Acrobatics, Skill.Athletics, Skill.Deception, Skill.Insight, Skill.Intimidation,
            Skill.Investigation, Skill.Perception, Skill.Performance, Skill.Persuasion,
            Skill.SleightOfHand, Skill.Stealth,
        ],
        ExpertiseChoices = 2,
    };

    private static Race SkilledHalfElf
    {
        get
        {
            var race = HalfElf;
            race.SkillChoices = 2;
            return race;
        }
    }

    private static Race Elf => new()
    {
        Name = "Эльф",
        Type = CharacterRaceType.Elf,
        Asi = [new Asi { Stat = "Ловкость", Value = 2, Ability = Ability.Dexterity }],
        Speed = [new Speed { Type = "Ходьба", Value = 30 }],
        Skills = [Skill.Perception],
    };

    /// <summary>
    /// Эльф-плут-преступник: Ловкость 15 + 2 = 17 (+3), Мудрость 12 (+1)
    /// </summary>
    private static CharacterDraft RogueDraft(
        Skill[]? classSkills = null, Skill[]? expertise = null, BackgroundType background = BackgroundType.Criminal) =>
        new(CharacterRaceType.Elf, CharacterClassType.Rogue, GenerationMethod.StandardArray,
            new AbilityScores(10, 15, 13, 14, 12, 8), [])
        {
            Background = background,
            ClassSkills = classSkills ?? [Skill.Acrobatics, Skill.Insight, Skill.Investigation, Skill.SleightOfHand],
            Expertise = expertise ?? [Skill.Stealth, Skill.SleightOfHand],
        };

    private static Dictionary<string, string[]> SkillViolation(
        CharacterDraft draft, Race? race = null, CharacterClass? characterClass = null, Background? background = null) =>
        Assert.Throws<CharacterRulesException>(() =>
                CharacterRules.Evaluate(draft, race ?? Elf, characterClass ?? Rogue, background ?? Criminal))
            .Errors.ToDictionary(x => x.Key, x => x.Value);

    [Fact]
    public void Бонусы_навыков_плута_с_компетентностью()
    {
        var sheet = CharacterRules.Evaluate(RogueDraft(), Elf, Rogue, Criminal);
        var skills = sheet.Skills.ToDictionary(x => x.Skill);

        // Ловкость +3, мастерство +2, компетентность ×2
        Assert.Equal(7, skills[Skill.Stealth].Bonus);
        Assert.True(skills[Skill.Stealth].Expertise);
        Assert.Equal(5, skills[Skill.Acrobatics].Bonus);
        Assert.False(skills[Skill.Acrobatics].Expertise);
        // Обман — от предыстории, Харизма 8 (−1) + 2
        Assert.Equal(1, skills[Skill.Deception].Bonus);
        // Атлетика без владения, Сила 10
        Assert.Equal(0, skills[Skill.Athletics].Bonus);
        Assert.False(skills[Skill.Athletics].Proficient);
        // Внимательность от расы: Мудрость 12 (+1) + 2 = 3, пассивная 13
        Assert.Equal(3, skills[Skill.Perception].Bonus);
        Assert.Equal(13, sheet.PassivePerception);
        Assert.Equal(Ability.Dexterity, skills[Skill.Stealth].Ability);
        Assert.Equal(18, sheet.Skills.Count);
    }

    [Fact]
    public void Навык_класса_повторяет_навык_предыстории()
    {
        var errors = SkillViolation(RogueDraft(classSkills: [Skill.Stealth, Skill.Insight, Skill.Investigation, Skill.Acrobatics]));

        Assert.Equal("Навык «Скрытность» уже даёт предыстория", errors["ClassSkills"].Single());
    }

    [Fact]
    public void Навык_класса_повторяет_навык_расы()
    {
        var errors = SkillViolation(RogueDraft(classSkills: [Skill.Perception, Skill.Insight, Skill.Investigation, Skill.Acrobatics]));

        Assert.Equal("Навык «Внимательность» уже даёт раса", errors["ClassSkills"].Single());
    }

    [Fact]
    public void Навык_не_из_списка_класса()
    {
        var errors = SkillViolation(RogueDraft(classSkills: [Skill.Arcana, Skill.Insight, Skill.Investigation, Skill.Acrobatics]));

        Assert.Contains("Магия", errors["ClassSkills"].Single());
    }

    [Fact]
    public void Неверное_число_навыков_класса()
    {
        var errors = SkillViolation(RogueDraft(classSkills: [Skill.Insight, Skill.Investigation]));

        Assert.Equal("Класс «Плут» выбирает 4 разных навыка, выбрано 2", errors["ClassSkills"].Single());
    }

    [Fact]
    public void Компетентность_только_во_владеемых_навыках()
    {
        var errors = SkillViolation(RogueDraft(expertise: [Skill.Stealth, Skill.Athletics]));

        Assert.Contains("Атлетика", errors["Expertise"].Single());
    }

    [Fact]
    public void Компетентность_не_у_плута_отклоняется()
    {
        var draft = new CharacterDraft(CharacterRaceType.Dwarf, CharacterClassType.Fighter, GenerationMethod.StandardArray,
            StandardBase, []) { Background = BackgroundType.Custom, Expertise = [Skill.Athletics] };

        var errors = Assert.Throws<CharacterRulesException>(() =>
            CharacterRules.Evaluate(draft, Dwarf, Fighter, NoBackground)).Errors;

        Assert.Equal("Класс «Fighter» не получает компетентность на 1 уровне", errors["Expertise"].Single());
    }

    [Fact]
    public void Своя_предыстория_выбирает_два_навыка()
    {
        var draft = RogueDraft(background: BackgroundType.Custom);
        var withChoices = new CharacterDraft(draft.Race, draft.Class, draft.Method, draft.BaseScores, [])
        {
            Background = BackgroundType.Custom,
            BackgroundSkills = [Skill.Arcana, Skill.History],
            ClassSkills = draft.ClassSkills,
            Expertise = [Skill.Arcana, Skill.SleightOfHand],
        };

        var sheet = CharacterRules.Evaluate(withChoices, Elf, Rogue, Custom);
        Assert.True(sheet.Skills.Single(x => x.Skill == Skill.Arcana).Expertise);

        var missing = withChoices with { BackgroundSkills = [Skill.Arcana] };
        var errors = SkillViolation(missing, background: Custom);
        Assert.Equal("Предыстория «Своя предыстория» выбирает 2 разных навыка, выбрано 1", errors["BackgroundSkills"].Single());
    }

    [Fact]
    public void Полуэльф_выбирает_два_любых_навыка()
    {
        var draft = new CharacterDraft(CharacterRaceType.HalfElf, CharacterClassType.Rogue, GenerationMethod.StandardArray,
            StandardBase, [Ability.Dexterity, Ability.Wisdom])
        {
            Background = BackgroundType.Criminal,
            RaceSkills = [Skill.Arcana, Skill.Medicine],
            ClassSkills = [Skill.Acrobatics, Skill.Insight, Skill.Investigation, Skill.Perception],
            Expertise = [Skill.Arcana, Skill.Stealth],
        };

        var sheet = CharacterRules.Evaluate(draft, SkilledHalfElf, Rogue, Criminal);
        Assert.Equal(8, sheet.Skills.Count(x => x.Proficient));

        var errors = SkillViolation(draft with { RaceSkills = [Skill.Stealth, Skill.Medicine] }, SkilledHalfElf);
        Assert.Equal("Навык «Скрытность» уже даёт предыстория", errors["RaceSkills"].Single());

        var noChoice = SkillViolation(draft with { RaceSkills = [] }, SkilledHalfElf);
        Assert.Equal("Полуэльф выбирает 2 разных навыка, выбрано 0", noChoice["RaceSkills"].Single());
    }
}
