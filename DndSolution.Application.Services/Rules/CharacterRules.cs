using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Backgrounds;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using DndSolution.Application.Models.Models.Rules;

namespace DndSolution.Application.Services.Rules;

/// <summary>
/// Правила создания персонажа 1 уровня по «Книге игрока» D&amp;D 5e
/// </summary>
public static class CharacterRules
{
    public const int Level = 1;
    public const int ProficiencyBonus = 2;
    public const int MaxScore = 20;
    public const int PointBuyBudget = 27;

    public static readonly IReadOnlyList<int> StandardArray = [15, 14, 13, 12, 10, 8];

    /// <summary>
    /// Стоимость значения при покупке очков (значения 8–15)
    /// </summary>
    public static readonly IReadOnlyDictionary<int, int> PointBuyCost = new Dictionary<int, int>
    {
        [8] = 0, [9] = 1, [10] = 2, [11] = 3, [12] = 4, [13] = 5, [14] = 7, [15] = 9,
    };

    public static readonly IReadOnlyDictionary<Ability, string> AbilityNames = new Dictionary<Ability, string>
    {
        [Ability.Strength] = "Сила",
        [Ability.Dexterity] = "Ловкость",
        [Ability.Constitution] = "Телосложение",
        [Ability.Intelligence] = "Интеллект",
        [Ability.Wisdom] = "Мудрость",
        [Ability.Charisma] = "Харизма",
    };

    public static readonly IReadOnlyDictionary<Skill, Ability> SkillAbilities = new Dictionary<Skill, Ability>
    {
        [Skill.Acrobatics] = Ability.Dexterity,
        [Skill.AnimalHandling] = Ability.Wisdom,
        [Skill.Arcana] = Ability.Intelligence,
        [Skill.Athletics] = Ability.Strength,
        [Skill.Deception] = Ability.Charisma,
        [Skill.History] = Ability.Intelligence,
        [Skill.Insight] = Ability.Wisdom,
        [Skill.Intimidation] = Ability.Charisma,
        [Skill.Investigation] = Ability.Intelligence,
        [Skill.Medicine] = Ability.Wisdom,
        [Skill.Nature] = Ability.Intelligence,
        [Skill.Perception] = Ability.Wisdom,
        [Skill.Performance] = Ability.Charisma,
        [Skill.Persuasion] = Ability.Charisma,
        [Skill.Religion] = Ability.Intelligence,
        [Skill.SleightOfHand] = Ability.Dexterity,
        [Skill.Stealth] = Ability.Dexterity,
        [Skill.Survival] = Ability.Wisdom,
    };

    public static readonly IReadOnlyDictionary<Skill, string> SkillNames = new Dictionary<Skill, string>
    {
        [Skill.Acrobatics] = "Акробатика",
        [Skill.AnimalHandling] = "Уход за животными",
        [Skill.Arcana] = "Магия",
        [Skill.Athletics] = "Атлетика",
        [Skill.Deception] = "Обман",
        [Skill.History] = "История",
        [Skill.Insight] = "Проницательность",
        [Skill.Intimidation] = "Запугивание",
        [Skill.Investigation] = "Анализ",
        [Skill.Medicine] = "Медицина",
        [Skill.Nature] = "Природа",
        [Skill.Perception] = "Внимательность",
        [Skill.Performance] = "Выступление",
        [Skill.Persuasion] = "Убеждение",
        [Skill.Religion] = "Религия",
        [Skill.SleightOfHand] = "Ловкость рук",
        [Skill.Stealth] = "Скрытность",
        [Skill.Survival] = "Выживание",
    };

    public static int Modifier(int score) => (int)Math.Floor((score - 10) / 2.0);

    /// <summary>
    /// Проверяет черновик и считает лист персонажа
    /// </summary>
    /// <exception cref="CharacterRulesException">Черновик нарушает правила</exception>
    public static CharacterSheet Evaluate(
        CharacterDraft draft, Race race, CharacterClass characterClass, Background background)
    {
        var errors = new Dictionary<string, List<string>>();
        void Error(string field, string message) =>
            (errors.TryGetValue(field, out var list) ? list : errors[field] = []).Add(message);

        ValidateGeneration(draft, message => Error("BaseStats", message));
        var bonuses = RacialBonuses(draft, race, message => Error("BonusChoices", message));
        var (proficient, expertise) = Skills(draft, race, characterClass, background, Error);

        if (errors.Count > 0)
        {
            throw new CharacterRulesException(errors.ToDictionary(e => e.Key, e => e.Value.ToArray()));
        }

        var scores = AbilityScores.From(a => Math.Min(MaxScore, draft.BaseScores[a] + bonuses.GetValueOrDefault(a)));
        var modifiers = AbilityScores.From(a => Modifier(scores[a]));
        var skills = Enum.GetValues<Skill>()
            .Select(skill =>
            {
                var ability = SkillAbilities[skill];
                var multiplier = expertise.Contains(skill) ? 2 : proficient.Contains(skill) ? 1 : 0;
                return new SkillCheck(skill, ability, modifiers[ability] + ProficiencyBonus * multiplier,
                    multiplier > 0, multiplier == 2);
            })
            .ToList();

        return new CharacterSheet(
            scores,
            modifiers,
            HitPoints: Math.Max(1, characterClass.HitDie + modifiers.Constitution),
            ArmorClass: UnarmoredArmorClass(characterClass.Type, modifiers),
            Initiative: modifiers.Dexterity,
            Speed: race.Speed.FirstOrDefault()?.Value ?? 30,
            ProficiencyBonus,
            SavingThrows: AbilityScores.All
                .Select(a =>
                {
                    var proficient = characterClass.SavingThrows.Contains(a);
                    return new SavingThrow(a, modifiers[a] + (proficient ? ProficiencyBonus : 0), proficient);
                })
                .ToList(),
            skills,
            PassivePerception: 10 + skills.Single(x => x.Skill == Skill.Perception).Bonus);
    }

    private static void ValidateGeneration(CharacterDraft draft, Action<string> error)
    {
        var values = draft.BaseScores.Values.ToList();
        switch (draft.Method)
        {
            case GenerationMethod.StandardArray:
                ValidatePermutation(values, StandardArray, "стандартного набора", error);
                break;

            case GenerationMethod.PointBuy:
                var outOfRange = values.Where(v => !PointBuyCost.ContainsKey(v)).Distinct().ToList();
                if (outOfRange.Count > 0)
                {
                    error($"При покупке очков значения должны быть от 8 до 15, а не {string.Join(", ", outOfRange)}");
                    break;
                }

                var spent = values.Sum(v => PointBuyCost[v]);
                if (spent > PointBuyBudget)
                {
                    error($"Потрачено {spent} очков из {PointBuyBudget}");
                }
                break;

            case GenerationMethod.Roll:
                if (draft.RolledValues is not { Count: 6 } rolled)
                {
                    error("Результат бросков недействителен или устарел, бросьте заново");
                    break;
                }

                ValidatePermutation(values, rolled, "бросков", error);
                break;

            default:
                error("Неизвестный способ генерации характеристик");
                break;
        }
    }

    private static void ValidatePermutation(List<int> values, IReadOnlyList<int> expected, string source, Action<string> error)
    {
        var left = expected.ToList();
        foreach (var value in values)
        {
            if (!left.Remove(value))
            {
                error(expected.Contains(value)
                    ? $"Значение {value} использовано больше раз, чем есть в результатах {source}"
                    : $"Значения {value} нет в результатах {source} ({string.Join(", ", expected)})");
            }
        }
    }

    private static Dictionary<Ability, int> RacialBonuses(CharacterDraft draft, Race race, Action<string> error)
    {
        var bonuses = race.Asi
            .Where(x => x.Ability is not null)
            .ToDictionary(x => x.Ability!.Value, x => x.Value);

        var choiceBonuses = race.Asi.Where(x => x.Ability is null).ToList();
        var expected = choiceBonuses.Sum(x => x.Choices);
        var choices = draft.BonusChoices;

        if (expected == 0)
        {
            if (choices.Count > 0)
            {
                error($"У расы «{race.Name}» нет бонусов характеристик на выбор");
            }

            return bonuses;
        }

        var fixedNames = string.Join(", ", bonuses.Keys.Select(a => AbilityNames[a]));
        var rule = $"{race.Name} выбирает {expected} разные характеристики" +
                   (bonuses.Count > 0 ? $", кроме: {fixedNames}" : "");
        if (choices.Count != expected || choices.Distinct().Count() != choices.Count
                                      || choices.Any(bonuses.ContainsKey)
                                      || choices.Any(c => !Enum.IsDefined(c)))
        {
            error(rule);
            return bonuses;
        }

        // Выборы распределяются по бонусам «на выбор» в порядке справочника
        var queue = new Queue<Ability>(choices);
        foreach (var bonus in choiceBonuses)
        {
            for (var i = 0; i < bonus.Choices; i++)
            {
                bonuses[queue.Dequeue()] = bonus.Value;
            }
        }

        return bonuses;
    }

    /// <summary>
    /// Проверяет выбор навыков в порядке шагов мастера: предыстория, раса, класс, компетентность
    /// </summary>
    private static (HashSet<Skill> Proficient, HashSet<Skill> Expertise) Skills(
        CharacterDraft draft, Race race, CharacterClass characterClass, Background background,
        Action<string, string> error)
    {
        var known = new HashSet<Skill>();
        var sources = new Dictionary<Skill, string>();

        void Grant(IEnumerable<Skill> skills, string source)
        {
            foreach (var skill in skills)
            {
                if (known.Add(skill))
                {
                    sources[skill] = source;
                }
            }
        }

        // Выбор: ровно count разных навыков из options, которых ещё нет
        void Choose(string field, IReadOnlyList<Skill> chosen, int count, IReadOnlyCollection<Skill> options,
            string who, string source)
        {
            if (chosen.Any(s => !Enum.IsDefined(s)))
            {
                error(field, "Неизвестный навык");
                return;
            }

            if (chosen.Count != count || chosen.Distinct().Count() != chosen.Count)
            {
                error(field, count == 0
                    ? $"{who} не выбирает навыки"
                    : $"{who} выбирает {count} разных навыка, выбрано {chosen.Distinct().Count()}");
                return;
            }

            foreach (var skill in chosen)
            {
                if (known.Contains(skill))
                {
                    error(field, $"Навык «{SkillNames[skill]}» уже даёт {sources[skill]}");
                }
                else if (!options.Contains(skill))
                {
                    error(field, $"Навыка «{SkillNames[skill]}» нет в списке: {who.ToLowerInvariant()}");
                }
            }

            Grant(chosen, source);
        }

        if (background.Type != draft.Background)
        {
            error("Background", "Неизвестная предыстория");
            return (known, []);
        }

        var allSkills = Enum.GetValues<Skill>();
        Grant(background.Skills, "предыстория");
        Choose("BackgroundSkills", draft.BackgroundSkills, background.SkillChoices, allSkills,
            $"Предыстория «{background.Name}»", "предыстория");

        Grant(race.Skills, "раса");
        Choose("RaceSkills", draft.RaceSkills, race.SkillChoices, allSkills, race.Name, "раса");

        Choose("ClassSkills", draft.ClassSkills, characterClass.SkillChoices, characterClass.SkillOptions,
            $"Класс «{characterClass.Name}»", "класс");

        var expertise = draft.Expertise.ToHashSet();
        if (draft.Expertise.Count != characterClass.ExpertiseChoices || expertise.Count != draft.Expertise.Count)
        {
            error("Expertise", characterClass.ExpertiseChoices == 0
                ? $"Класс «{characterClass.Name}» не получает компетентность на 1 уровне"
                : $"Компетентность получают {characterClass.ExpertiseChoices} разных навыка");
        }
        else if (draft.Expertise.Where(s => !known.Contains(s)).ToList() is { Count: > 0 } notKnown)
        {
            error("Expertise", "Компетентность — только в навыках, которыми персонаж владеет, а не в: " +
                               string.Join(", ", notKnown.Select(s => SkillNames.GetValueOrDefault(s, "?"))));
        }

        return (known, expertise);
    }

    private static int UnarmoredArmorClass(CharacterClassType type, AbilityScores modifiers) => type switch
    {
        CharacterClassType.Barbarian => 10 + modifiers.Dexterity + modifiers.Constitution,
        CharacterClassType.Monk => 10 + modifiers.Dexterity + modifiers.Wisdom,
        _ => 10 + modifiers.Dexterity,
    };
}
