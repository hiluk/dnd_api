using DndSolution.Application.Models.Enums;
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
