using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Backgrounds;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;

namespace Data.Repositories;

/// <summary>
/// Расы и классы из «Книги игрока» D&amp;D 5e (официальный перевод)
/// </summary>
/// <remarks>
/// Порядок записей совпадает с номерами в enum'ах CharacterRaceType и CharacterClassType,
/// а названия — с клиентскими: клиент сопоставляет их по названию.
/// </remarks>
internal static class CatalogSeed
{
    private const string Walk = "Ходьба";
    private const string Medium = "Средний";
    private const string Small = "Маленький";
    private const string Darkvision = "Тёмное зрение, 60 футов";
    private const string NormalVision = "Обычное";

    /// <summary>
    /// Старые названия, которые нужно переименовать в официальные
    /// </summary>
    public static readonly IReadOnlyDictionary<string, string> ClassRenames = new Dictionary<string, string>
    {
        ["Рейнджер"] = "Следопыт",
        ["Вор"] = "Плут",
    };

    public static IReadOnlyList<Race> Races =>
    [
        new Race
        {
            Name = "Человек",
            Type = CharacterRaceType.Human,
            Description = "Самый молодой и самый распространённый народ. Люди амбициозны, изобретательны и легко приспосабливаются к любым условиям.",
            Age = "Взрослеют к 20 годам и редко живут дольше столетия.",
            Size = Medium,
            Language = "Общий и ещё один язык на выбор.",
            Vision = NormalVision,
            Traits = "Значения всех характеристик увеличиваются на 1.",
            Asi = [Asi("Сила", 1), Asi("Ловкость", 1), Asi("Телосложение", 1), Asi("Интеллект", 1), Asi("Мудрость", 1), Asi("Харизма", 1)],
            Speed = Speed(30),
        },
        new Race
        {
            Name = "Полуэльф",
            Type = CharacterRaceType.HalfElf,
            SkillChoices = 2,
            Description = "Полуэльфы сочетают лучшие черты людей и эльфов: любознательность и амбиции одних с утончённостью и обаянием других.",
            Age = "Взрослеют к 20 годам, живут более 180 лет.",
            Size = Medium,
            Language = "Общий, эльфийский и ещё один язык на выбор.",
            Vision = Darkvision,
            Traits = "Наследие фей: преимущество на спасброски от очарования, магия не может усыпить. Универсальность навыков: владение двумя навыками на выбор.",
            Asi = [Asi("Харизма", 2), new Asi { Stat = "Две другие на выбор", Value = 1, Choices = 2 }],
            Speed = Speed(30),
        },
        new Race
        {
            Name = "Дварф",
            Type = CharacterRaceType.Dwarf,
            Description = "Отважные и выносливые жители гор и подземных крепостей, искусные воины, рудокопы и мастера по камню и металлу.",
            Age = "Взрослеют к 50 годам, живут около 350 лет.",
            Size = Medium,
            Language = "Общий и дварфийский.",
            Vision = Darkvision,
            Traits = "Дварфийская устойчивость к яду. Боевая подготовка дварфов: боевые топоры, ручные топоры, лёгкие и боевые молоты. Знание камня. Скорость не снижается от тяжёлых доспехов.",
            Asi = [Asi("Телосложение", 2)],
            Speed = Speed(25),
        },
        new Race
        {
            Name = "Эльф",
            Type = CharacterRaceType.Elf,
            Skills = [Skill.Perception],
            Description = "Волшебный народ неземного изящества, живущий в мире, но не вполне принадлежащий ему.",
            Age = "Считаются взрослыми около 100 лет, живут до 750 лет.",
            Size = Medium,
            Language = "Общий и эльфийский.",
            Vision = Darkvision,
            Traits = "Обострённые чувства: владение Внимательностью. Наследие фей. Транс: вместо сна медитируют 4 часа.",
            Asi = [Asi("Ловкость", 2)],
            Speed = Speed(30),
        },
        new Race
        {
            Name = "Полурослик",
            Type = CharacterRaceType.Halfling,
            Description = "Небольшой и дружелюбный народ, ценящий домашний уют, хорошую еду и спокойную жизнь, но способный на удивительную храбрость.",
            Age = "Взрослеют к 20 годам, живут до середины второго столетия.",
            Size = Small,
            Language = "Общий и язык полуросликов.",
            Vision = NormalVision,
            Traits = "Везучий: можно перебросить 1 на к20. Храбрый: преимущество на спасброски от испуга. Проворство полуросликов: можно проходить сквозь существ крупнее себя.",
            Asi = [Asi("Ловкость", 2)],
            Speed = Speed(25),
        },
        new Race
        {
            Name = "Драконорождённый",
            Type = CharacterRaceType.Dragonborn,
            Description = "Гордые потомки драконов, внешне похожие на гуманоидных драконов, без крыльев и хвоста.",
            Age = "Взрослеют к 15 годам, живут около 80 лет.",
            Size = Medium,
            Language = "Общий и драконий.",
            Vision = NormalVision,
            Traits = "Наследие драконов: вид дракона определяет оружие дыхания и сопротивление урону. Оружие дыхания.",
            Asi = [Asi("Сила", 2), Asi("Харизма", 1)],
            Speed = Speed(30),
        },
        new Race
        {
            Name = "Гном",
            Type = CharacterRaceType.Gnome,
            Description = "Энергичные изобретатели и исследователи, полные любопытства и жизнелюбия.",
            Age = "Взрослеют к 40 годам, живут от 350 до почти 500 лет.",
            Size = Small,
            Language = "Общий и гномий.",
            Vision = Darkvision,
            Traits = "Гномья хитрость: преимущество на спасброски Интеллекта, Мудрости и Харизмы против магии.",
            Asi = [Asi("Интеллект", 2)],
            Speed = Speed(25),
        },
        new Race
        {
            Name = "Полуорк",
            Type = CharacterRaceType.HalfOrc,
            Skills = [Skill.Intimidation],
            Description = "Потомки людей и орков, сочетающие человеческую решимость с орочьей силой и яростью.",
            Age = "Взрослеют к 14 годам, редко живут дольше 75 лет.",
            Size = Medium,
            Language = "Общий и орочий.",
            Vision = Darkvision,
            Traits = "Угрожающий вид: владение Запугиванием. Непоколебимая стойкость: раз в день при падении до 0 хитов остаётся 1. Свирепые атаки: дополнительная кость урона при критическом ударе.",
            Asi = [Asi("Сила", 2), Asi("Телосложение", 1)],
            Speed = Speed(30),
        },
        new Race
        {
            Name = "Тифлинг",
            Type = CharacterRaceType.Tiefling,
            Description = "Люди, несущие на себе печать договора с силами Девяти Преисподних: рога, хвост и глаза без зрачков.",
            Age = "Взрослеют как люди, живут чуть дольше.",
            Size = Medium,
            Language = "Общий и инфернальный.",
            Vision = Darkvision,
            Traits = "Адское сопротивление: сопротивление урону огнём. Дьявольское наследие: знают заклинание «Чудотворство», позже «Адское возмездие» и «Тьма».",
            Asi = [Asi("Харизма", 2), Asi("Интеллект", 1)],
            Speed = Speed(30),
        },
    ];

    public static IReadOnlyList<CharacterClass> Classes =>
    [
        new CharacterClass
        {
            Name = "Воин",
            Type = CharacterClassType.Fighter,
            SkillChoices = 2,
            SkillOptions = [Skill.Acrobatics, Skill.AnimalHandling, Skill.Athletics, Skill.History, Skill.Insight, Skill.Intimidation, Skill.Perception, Skill.Survival],
            HitDie = 10,
            SavingThrows = [Ability.Strength, Ability.Constitution],
            Description = "Мастер боевых искусств, искусно владеющий любым оружием и доспехами.",
            HitDice = "1к10",
            HpAt1stLevel = Hp1(10),
            HpAtHigherLevels = HpHigher(10),
            ProfArmor = "Все доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Сила, Телосложение",
            ProfSkills = "Два навыка из: Акробатика, Атлетика, Внимательность, Выживание, Запугивание, История, Проницательность, Уход за животными",
            Equipment = "Кольчуга или кожаный доспех с длинным луком; воинское оружие и щит или два воинских оружия; лёгкий арбалет или два ручных топора; набор исследователя подземелий или путешественника",
            SpellCastingAbility = "Нет",
        },
        new CharacterClass
        {
            Name = "Варвар",
            Type = CharacterClassType.Barbarian,
            SkillChoices = 2,
            SkillOptions = [Skill.AnimalHandling, Skill.Athletics, Skill.Intimidation, Skill.Nature, Skill.Perception, Skill.Survival],
            HitDie = 12,
            SavingThrows = [Ability.Strength, Ability.Constitution],
            Description = "Свирепый воин примитивного происхождения, способный впадать в боевую ярость.",
            HitDice = "1к12",
            HpAt1stLevel = Hp1(12),
            HpAtHigherLevels = HpHigher(12),
            ProfArmor = "Лёгкие и средние доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Сила, Телосложение",
            ProfSkills = "Два навыка из: Атлетика, Внимательность, Выживание, Запугивание, Природа, Уход за животными",
            Equipment = "Секира или любое воинское рукопашное оружие; два ручных топора или любое простое оружие; набор путешественника и четыре метательных копья",
            SpellCastingAbility = "Нет",
        },
        new CharacterClass
        {
            Name = "Следопыт",
            Type = CharacterClassType.Ranger,
            SkillChoices = 3,
            SkillOptions = [Skill.AnimalHandling, Skill.Athletics, Skill.Insight, Skill.Investigation, Skill.Nature, Skill.Perception, Skill.Stealth, Skill.Survival],
            HitDie = 10,
            SavingThrows = [Ability.Strength, Ability.Dexterity],
            Description = "Воин, использующий боевые навыки и магию природы для борьбы с угрозами на границах цивилизации.",
            HitDice = "1к10",
            HpAt1stLevel = Hp1(10),
            HpAtHigherLevels = HpHigher(10),
            ProfArmor = "Лёгкие и средние доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Сила, Ловкость",
            ProfSkills = "Три навыка из: Анализ, Атлетика, Внимательность, Выживание, Природа, Проницательность, Скрытность, Уход за животными",
            Equipment = "Чешуйчатый или кожаный доспех; два коротких меча или два простых рукопашных оружия; набор исследователя подземелий или путешественника; длинный лук и колчан с 20 стрелами",
            SpellCastingAbility = "Мудрость",
        },
        new CharacterClass
        {
            Name = "Плут",
            Type = CharacterClassType.Rogue,
            SkillChoices = 4,
            SkillOptions = [Skill.Acrobatics, Skill.Athletics, Skill.Deception, Skill.Insight, Skill.Intimidation, Skill.Investigation, Skill.Perception, Skill.Performance, Skill.Persuasion, Skill.SleightOfHand, Skill.Stealth],
            ExpertiseChoices = 2,
            HitDie = 8,
            SavingThrows = [Ability.Dexterity, Ability.Intelligence],
            Description = "Ловкий и хитрый мошенник, полагающийся на скрытность, хитрость и точные удары.",
            HitDice = "1к8",
            HpAt1stLevel = Hp1(8),
            HpAtHigherLevels = HpHigher(8),
            ProfArmor = "Лёгкие доспехи",
            ProfWeapon = "Простое оружие, ручные арбалеты, длинные мечи, рапиры, короткие мечи",
            ProfTool = "Воровские инструменты",
            ProfSavingThrows = "Ловкость, Интеллект",
            ProfSkills = "Четыре навыка из: Акробатика, Анализ, Атлетика, Внимательность, Выступление, Запугивание, Ловкость рук, Обман, Проницательность, Скрытность, Убеждение",
            Equipment = "Рапира или короткий меч; короткий лук с колчаном и 20 стрелами или короткий меч; набор взломщика, исследователя подземелий или путешественника; кожаный доспех, два кинжала и воровские инструменты",
            SpellCastingAbility = "Нет",
        },
        new CharacterClass
        {
            Name = "Бард",
            Type = CharacterClassType.Bard,
            SkillChoices = 3,
            SkillOptions = Enum.GetValues<Skill>().ToList(),
            HitDie = 8,
            SavingThrows = [Ability.Dexterity, Ability.Charisma],
            Description = "Вдохновляющий маг, чья сила звучит в музыке и словах творения.",
            HitDice = "1к8",
            HpAt1stLevel = Hp1(8),
            HpAtHigherLevels = HpHigher(8),
            ProfArmor = "Лёгкие доспехи",
            ProfWeapon = "Простое оружие, ручные арбалеты, длинные мечи, рапиры, короткие мечи",
            ProfTool = "Три музыкальных инструмента на выбор",
            ProfSavingThrows = "Ловкость, Харизма",
            ProfSkills = "Три любых навыка",
            Equipment = "Рапира, длинный меч или простое оружие; набор дипломата или артиста; лютня или другой музыкальный инструмент; кожаный доспех и кинжал",
            SpellCastingAbility = "Харизма",
        },
        new CharacterClass
        {
            Name = "Жрец",
            Type = CharacterClassType.Cleric,
            SkillChoices = 2,
            SkillOptions = [Skill.History, Skill.Insight, Skill.Medicine, Skill.Persuasion, Skill.Religion],
            HitDie = 8,
            SavingThrows = [Ability.Wisdom, Ability.Charisma],
            Description = "Жреческий поборник, владеющий божественной магией во имя высших сил.",
            HitDice = "1к8",
            HpAt1stLevel = Hp1(8),
            HpAtHigherLevels = HpHigher(8),
            ProfArmor = "Лёгкие и средние доспехи, щиты",
            ProfWeapon = "Простое оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Мудрость, Харизма",
            ProfSkills = "Два навыка из: История, Медицина, Проницательность, Религия, Убеждение",
            Equipment = "Булава или боевой молот; чешуйчатый, кожаный или кольчужный доспех; лёгкий арбалет с 20 болтами или простое оружие; набор священника или путешественника; щит и священный символ",
            SpellCastingAbility = "Мудрость",
        },
        new CharacterClass
        {
            Name = "Друид",
            Type = CharacterClassType.Druid,
            SkillChoices = 2,
            SkillOptions = [Skill.Arcana, Skill.AnimalHandling, Skill.Insight, Skill.Medicine, Skill.Nature, Skill.Perception, Skill.Religion, Skill.Survival],
            HitDie = 8,
            SavingThrows = [Ability.Intelligence, Ability.Wisdom],
            Description = "Жрец Старой Веры, повелевающий силами природы и принимающий облик животных.",
            HitDice = "1к8",
            HpAt1stLevel = Hp1(8),
            HpAtHigherLevels = HpHigher(8),
            ProfArmor = "Лёгкие и средние доспехи, щиты (не из металла)",
            ProfWeapon = "Боевые посохи, булавы, дротики, дубинки, кинжалы, копья, метательные копья, пращи, серпы, скимитары",
            ProfTool = "Набор травника",
            ProfSavingThrows = "Интеллект, Мудрость",
            ProfSkills = "Два навыка из: Внимательность, Выживание, Магия, Медицина, Природа, Проницательность, Религия, Уход за животными",
            Equipment = "Деревянный щит или простое оружие; скимитар или простое рукопашное оружие; кожаный доспех, набор путешественника и друидическая фокусировка",
            SpellCastingAbility = "Мудрость",
        },
        new CharacterClass
        {
            Name = "Монах",
            Type = CharacterClassType.Monk,
            SkillChoices = 2,
            SkillOptions = [Skill.Acrobatics, Skill.Athletics, Skill.History, Skill.Insight, Skill.Religion, Skill.Stealth],
            HitDie = 8,
            SavingThrows = [Ability.Strength, Ability.Dexterity],
            Description = "Мастер боевых искусств, использующий силу тела и духа — ци — для достижения совершенства.",
            HitDice = "1к8",
            HpAt1stLevel = Hp1(8),
            HpAtHigherLevels = HpHigher(8),
            ProfArmor = "Нет",
            ProfWeapon = "Простое оружие, короткие мечи",
            ProfTool = "Один вид инструментов ремесленника или один музыкальный инструмент",
            ProfSavingThrows = "Сила, Ловкость",
            ProfSkills = "Два навыка из: Акробатика, Атлетика, История, Проницательность, Религия, Скрытность",
            Equipment = "Короткий меч или простое оружие; набор исследователя подземелий или путешественника; 10 дротиков",
            SpellCastingAbility = "Нет",
        },
        new CharacterClass
        {
            Name = "Паладин",
            Type = CharacterClassType.Paladin,
            SkillChoices = 2,
            SkillOptions = [Skill.Athletics, Skill.Insight, Skill.Intimidation, Skill.Medicine, Skill.Persuasion, Skill.Religion],
            HitDie = 10,
            SavingThrows = [Ability.Wisdom, Ability.Charisma],
            Description = "Святой воин, связанный священной клятвой.",
            HitDice = "1к10",
            HpAt1stLevel = Hp1(10),
            HpAtHigherLevels = HpHigher(10),
            ProfArmor = "Все доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Мудрость, Харизма",
            ProfSkills = "Два навыка из: Атлетика, Запугивание, Медицина, Проницательность, Религия, Убеждение",
            Equipment = "Воинское оружие и щит или два воинских оружия; пять метательных копий или простое рукопашное оружие; набор священника или путешественника; кольчуга и священный символ",
            SpellCastingAbility = "Харизма",
        },
        new CharacterClass
        {
            Name = "Чародей",
            Type = CharacterClassType.Sorcerer,
            SkillChoices = 2,
            SkillOptions = [Skill.Arcana, Skill.Deception, Skill.Insight, Skill.Intimidation, Skill.Persuasion, Skill.Religion],
            HitDie = 6,
            SavingThrows = [Ability.Constitution, Ability.Charisma],
            Description = "Заклинатель, черпающий магию из дара или родословной.",
            HitDice = "1к6",
            HpAt1stLevel = Hp1(6),
            HpAtHigherLevels = HpHigher(6),
            ProfArmor = "Нет",
            ProfWeapon = "Боевые посохи, дротики, кинжалы, лёгкие арбалеты, пращи",
            ProfTool = "Нет",
            ProfSavingThrows = "Телосложение, Харизма",
            ProfSkills = "Два навыка из: Запугивание, Магия, Обман, Проницательность, Религия, Убеждение",
            Equipment = "Лёгкий арбалет с 20 болтами или простое оружие; мешочек с компонентами или магическая фокусировка; набор исследователя подземелий или путешественника; два кинжала",
            SpellCastingAbility = "Харизма",
        },
        new CharacterClass
        {
            Name = "Колдун",
            Type = CharacterClassType.Warlock,
            SkillChoices = 2,
            SkillOptions = [Skill.Arcana, Skill.Deception, Skill.History, Skill.Intimidation, Skill.Investigation, Skill.Nature, Skill.Religion],
            HitDie = 8,
            SavingThrows = [Ability.Wisdom, Ability.Charisma],
            Description = "Владелец магии, полученной в результате сделки с потусторонним покровителем.",
            HitDice = "1к8",
            HpAt1stLevel = Hp1(8),
            HpAtHigherLevels = HpHigher(8),
            ProfArmor = "Лёгкие доспехи",
            ProfWeapon = "Простое оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Мудрость, Харизма",
            ProfSkills = "Два навыка из: Анализ, Запугивание, История, Магия, Обман, Природа, Религия",
            Equipment = "Лёгкий арбалет с 20 болтами или простое оружие; мешочек с компонентами или магическая фокусировка; набор учёного или исследователя подземелий; кожаный доспех, простое оружие и два кинжала",
            SpellCastingAbility = "Харизма",
        },
        new CharacterClass
        {
            Name = "Волшебник",
            Type = CharacterClassType.Wizard,
            SkillChoices = 2,
            SkillOptions = [Skill.Arcana, Skill.History, Skill.Insight, Skill.Investigation, Skill.Medicine, Skill.Religion],
            HitDie = 6,
            SavingThrows = [Ability.Intelligence, Ability.Wisdom],
            Description = "Учёный-маг, способный управлять структурами реальности.",
            HitDice = "1к6",
            HpAt1stLevel = Hp1(6),
            HpAtHigherLevels = HpHigher(6),
            ProfArmor = "Нет",
            ProfWeapon = "Боевые посохи, дротики, кинжалы, лёгкие арбалеты, пращи",
            ProfTool = "Нет",
            ProfSavingThrows = "Интеллект, Мудрость",
            ProfSkills = "Два навыка из: Анализ, История, Магия, Медицина, Проницательность, Религия",
            Equipment = "Боевой посох или кинжал; мешочек с компонентами или магическая фокусировка; набор учёного или исследователя подземелий; книга заклинаний",
            SpellCastingAbility = "Интеллект",
        },
    ];

    public static IReadOnlyList<Background> Backgrounds =>
    [
        new Background
        {
            Type = BackgroundType.Acolyte,
            Name = "Прислужник",
            Description = "Вы провели жизнь на службе у храма, совершая обряды и помогая верующим.",
            Skills = [Skill.Insight, Skill.Religion],
            Tools = "Нет",
            Languages = "Два на выбор",
            Equipment = "Священный символ, молитвенник, 5 палочек благовоний, облачение, обычная одежда, 15 зм",
            Feature = "Приют верующих: единоверцы дают кров, лечение и поддержку",
        },
        new Background
        {
            Type = BackgroundType.Charlatan,
            Name = "Шарлатан",
            Description = "Вы всегда умели находить подход к людям и знали, что им сказать, чтобы получить своё.",
            Skills = [Skill.Deception, Skill.SleightOfHand],
            Tools = "Набор для грима, набор для фальсификации",
            Languages = "Нет",
            Equipment = "Красивая одежда, набор для грима, инструменты для мошенничества, 15 зм",
            Feature = "Фальшивая личность: вторая личность с документами и знакомствами",
        },
        new Background
        {
            Type = BackgroundType.Criminal,
            Name = "Преступник",
            Description = "Вы опытный нарушитель закона с налаженными связями в преступном мире.",
            Skills = [Skill.Deception, Skill.Stealth],
            Tools = "Один игровой набор, воровские инструменты",
            Languages = "Нет",
            Equipment = "Ломик, тёмная одежда с капюшоном, 15 зм",
            Feature = "Криминальные связи: надёжный контакт в преступном мире",
        },
        new Background
        {
            Type = BackgroundType.Entertainer,
            Name = "Артист",
            Description = "Вы живёте ради аплодисментов и умеете захватить внимание любой публики.",
            Skills = [Skill.Acrobatics, Skill.Performance],
            Tools = "Набор для грима, один музыкальный инструмент",
            Languages = "Нет",
            Equipment = "Музыкальный инструмент, подарок поклонника, костюм, 15 зм",
            Feature = "По просьбе публики: выступления дают кров и пищу",
        },
        new Background
        {
            Type = BackgroundType.FolkHero,
            Name = "Народный герой",
            Description = "Вы вышли из простого народа, но вам суждено нечто большее.",
            Skills = [Skill.AnimalHandling, Skill.Survival],
            Tools = "Один вид инструментов ремесленника, наземный транспорт",
            Languages = "Нет",
            Equipment = "Инструменты ремесленника, лопата, железный котелок, обычная одежда, 10 зм",
            Feature = "Деревенское гостеприимство: простой люд укроет и поддержит",
        },
        new Background
        {
            Type = BackgroundType.GuildArtisan,
            Name = "Гильдейский ремесленник",
            Description = "Вы член ремесленной гильдии, мастер своего дела.",
            Skills = [Skill.Insight, Skill.Persuasion],
            Tools = "Один вид инструментов ремесленника",
            Languages = "Один на выбор",
            Equipment = "Инструменты ремесленника, рекомендательное письмо гильдии, одежда путешественника, 15 зм",
            Feature = "Членство в гильдии: помощь и защита собратьев по цеху",
        },
        new Background
        {
            Type = BackgroundType.Hermit,
            Name = "Отшельник",
            Description = "Долгие годы вы жили в уединении и узнали то, что скрыто от других.",
            Skills = [Skill.Medicine, Skill.Religion],
            Tools = "Набор травника",
            Languages = "Один на выбор",
            Equipment = "Футляр со свитками записей, зимнее одеяло, обычная одежда, набор травника, 5 зм",
            Feature = "Открытие: уникальное знание, найденное в уединении",
        },
        new Background
        {
            Type = BackgroundType.Noble,
            Name = "Благородный",
            Description = "Вы знаете, что такое богатство, власть и привилегии.",
            Skills = [Skill.History, Skill.Persuasion],
            Tools = "Один игровой набор",
            Languages = "Один на выбор",
            Equipment = "Отличная одежда, кольцо-печатка, свиток с родословной, 25 зм",
            Feature = "Привилегированность: вас принимают в высшем обществе",
        },
        new Background
        {
            Type = BackgroundType.Outlander,
            Name = "Чужеземец",
            Description = "Вы выросли вдали от цивилизации, в глуши и на просторах дикой природы.",
            Skills = [Skill.Athletics, Skill.Survival],
            Tools = "Один музыкальный инструмент",
            Languages = "Один на выбор",
            Equipment = "Посох, охотничья ловушка, трофей с убитого зверя, одежда путешественника, 10 зм",
            Feature = "Странник: отличная память на местность, вы всегда найдёте пищу",
        },
        new Background
        {
            Type = BackgroundType.Sage,
            Name = "Мудрец",
            Description = "Годы учёбы сделали вас знатоком древних знаний и преданий.",
            Skills = [Skill.Arcana, Skill.History],
            Tools = "Нет",
            Languages = "Два на выбор",
            Equipment = "Бутылочка чернил, перо, нож, письмо от умершего коллеги, обычная одежда, 10 зм",
            Feature = "Исследователь: вы знаете, где найти нужные сведения",
        },
        new Background
        {
            Type = BackgroundType.Sailor,
            Name = "Моряк",
            Description = "Годы вы провели на палубе корабля, лицом к лицу со штормами и морскими чудищами.",
            Skills = [Skill.Athletics, Skill.Perception],
            Tools = "Инструменты навигатора, водный транспорт",
            Languages = "Нет",
            Equipment = "Страховочная верёвка (50 футов), талисман, обычная одежда, 10 зм",
            Feature = "Корабельный пассаж: бесплатный проезд на кораблях для вас и спутников",
        },
        new Background
        {
            Type = BackgroundType.Soldier,
            Name = "Солдат",
            Description = "Война была вашей жизнью: вы служили в армии и знаете цену дисциплине.",
            Skills = [Skill.Athletics, Skill.Intimidation],
            Tools = "Один игровой набор, наземный транспорт",
            Languages = "Нет",
            Equipment = "Знак отличия, трофей с поверженного врага, игральные кости или колода карт, обычная одежда, 10 зм",
            Feature = "Воинское звание: солдаты признают ваш чин",
        },
        new Background
        {
            Type = BackgroundType.Urchin,
            Name = "Беспризорник",
            Description = "Вы выросли на улицах в одиночестве и бедности и научились выживать сами.",
            Skills = [Skill.SleightOfHand, Skill.Stealth],
            Tools = "Набор для грима, воровские инструменты",
            Languages = "Нет",
            Equipment = "Маленький нож, карта родного города, ручная мышь, памятная вещица, обычная одежда, 10 зм",
            Feature = "Городские тайны: вы знаете короткие пути через город",
        },
        new Background
        {
            Type = BackgroundType.Custom,
            Name = "Своя предыстория",
            Description = "Придумайте собственную историю персонажа и выберите два любых навыка.",
            SkillChoices = 2,
            Tools = "Два инструмента или языка на выбор (согласуйте с мастером)",
            Languages = "См. инструменты",
            Equipment = "Согласуйте с мастером",
            Feature = "Выберите умение одной из предысторий",
        },
    ];

    private static readonly Dictionary<string, Ability> AbilityByName = new()
    {
        ["Сила"] = Ability.Strength,
        ["Ловкость"] = Ability.Dexterity,
        ["Телосложение"] = Ability.Constitution,
        ["Интеллект"] = Ability.Intelligence,
        ["Мудрость"] = Ability.Wisdom,
        ["Харизма"] = Ability.Charisma,
    };

    private static Asi Asi(string stat, int value) =>
        new() { Stat = stat, Value = value, Ability = AbilityByName[stat] };

    private static List<Speed> Speed(int walk) => [new Speed { Type = Walk, Value = walk }];

    private static string Hp1(int dice) => $"{dice} + модификатор Телосложения";

    private static string HpHigher(int dice) =>
        $"1к{dice} (или {dice / 2 + 1}) + модификатор Телосложения за каждый уровень после первого";
}
