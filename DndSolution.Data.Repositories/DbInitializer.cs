using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

/// <summary>
/// Применение миграций и заполнение справочников рас и классов
/// </summary>
/// <remarks>
/// Клиент сопоставляет значения enum с записями справочника по названию,
/// поэтому названия здесь должны совпадать с клиентскими.
/// </remarks>
public static class DbInitializer
{
    public static async Task InitializeDbAsync(this IServiceProvider services, CancellationToken token = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DndContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DndContext>>();

        await context.Database.MigrateAsync(token);

        var existingRaces = await context.Races.Select(x => x.Name).ToListAsync(token);
        var newRaces = Races.Where(x => !existingRaces.Contains(x.Name)).ToList();
        await context.Races.AddRangeAsync(newRaces, token);

        var existingClasses = await context.Classes.Select(x => x.Name).ToListAsync(token);
        var newClasses = Classes.Where(x => !existingClasses.Contains(x.Name)).ToList();
        await context.Classes.AddRangeAsync(newClasses, token);

        await context.SaveChangesAsync(token);

        if (newRaces.Count + newClasses.Count > 0)
        {
            logger.LogInformation("Добавлено рас: {Races}, классов: {Classes}", newRaces.Count, newClasses.Count);
        }
    }

    private static IEnumerable<Race> Races =>
    [
        new Race
        {
            Name = "Человек",
            Description = "Самая молодая и самая распространённая из рас. Люди амбициозны, изобретательны и легко приспосабливаются к любым условиям.",
            Age = "Взрослеют к 20 годам и редко живут дольше столетия.",
            Size = "Средний",
            Language = "Общий и ещё один язык на выбор.",
            Vision = "Обычное",
            Traits = "Увеличение всех характеристик на 1.",
            Asi =
            [
                new Asi { Stat = "Сила", Value = 1 },
                new Asi { Stat = "Ловкость", Value = 1 },
                new Asi { Stat = "Телосложение", Value = 1 },
                new Asi { Stat = "Интеллект", Value = 1 },
                new Asi { Stat = "Мудрость", Value = 1 },
                new Asi { Stat = "Харизма", Value = 1 },
            ],
            Speed = [new Speed { Type = "Ходьба", Value = 30 }],
        },
        new Race
        {
            Name = "Полуэльф",
            Description = "Полуэльфы сочетают лучшие черты людей и эльфов: любознательность и амбиции одних с утончённостью и обаянием других.",
            Age = "Взрослеют к 20 годам, живут более 180 лет.",
            Size = "Средний",
            Language = "Общий, эльфийский и ещё один язык на выбор.",
            Vision = "Тёмное зрение, 60 футов",
            Traits = "Наследие фей, владение двумя навыками на выбор.",
            Asi =
            [
                new Asi { Stat = "Харизма", Value = 2 },
                new Asi { Stat = "Любые две", Value = 1 },
            ],
            Speed = [new Speed { Type = "Ходьба", Value = 30 }],
        },
    ];

    private static IEnumerable<CharacterClass> Classes =>
    [
        new CharacterClass
        {
            Name = "Воин",
            Description = "Мастер боевых искусств, искусно владеющий любым оружием и доспехами.",
            HitDice = "1к10",
            HpAt1stLevel = "10 + модификатор Телосложения",
            HpAtHigherLevels = "1к10 (или 6) + модификатор Телосложения за каждый уровень после первого",
            ProfArmor = "Все доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Сила, Телосложение",
            ProfSkills = "Два навыка из: Акробатика, Атлетика, Восприятие, Выживание, Запугивание, История, Проницательность, Уход за животными",
            Equipment = "Кольчуга, воинское оружие и щит, лёгкий арбалет, набор исследователя подземелий",
            SpellCastingAbility = "Нет",
        },
        new CharacterClass
        {
            Name = "Варвар",
            Description = "Свирепый воин, способный впадать в боевую ярость.",
            HitDice = "1к12",
            HpAt1stLevel = "12 + модификатор Телосложения",
            HpAtHigherLevels = "1к12 (или 7) + модификатор Телосложения за каждый уровень после первого",
            ProfArmor = "Лёгкие и средние доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Сила, Телосложение",
            ProfSkills = "Два навыка из: Атлетика, Восприятие, Выживание, Запугивание, Природа, Уход за животными",
            Equipment = "Секира, два ручных топора, набор путешественника, четыре метательных копья",
            SpellCastingAbility = "Нет",
        },
        new CharacterClass
        {
            Name = "Рейнджер",
            Description = "Охотник и следопыт, сражающийся с угрозами на границах цивилизации.",
            HitDice = "1к10",
            HpAt1stLevel = "10 + модификатор Телосложения",
            HpAtHigherLevels = "1к10 (или 6) + модификатор Телосложения за каждый уровень после первого",
            ProfArmor = "Лёгкие и средние доспехи, щиты",
            ProfWeapon = "Простое и воинское оружие",
            ProfTool = "Нет",
            ProfSavingThrows = "Сила, Ловкость",
            ProfSkills = "Три навыка из: Атлетика, Внимательность, Выживание, Природа, Проницательность, Расследование, Скрытность, Уход за животными",
            Equipment = "Чешуйчатый доспех, два коротких меча, длинный лук и колчан с 20 стрелами",
            SpellCastingAbility = "Мудрость",
        },
        new CharacterClass
        {
            Name = "Вор",
            Description = "Ловкий и хитрый плут, полагающийся на скрытность и точные удары.",
            HitDice = "1к8",
            HpAt1stLevel = "8 + модификатор Телосложения",
            HpAtHigherLevels = "1к8 (или 5) + модификатор Телосложения за каждый уровень после первого",
            ProfArmor = "Лёгкие доспехи",
            ProfWeapon = "Простое оружие, ручные арбалеты, длинные мечи, рапиры, короткие мечи",
            ProfTool = "Воровские инструменты",
            ProfSavingThrows = "Ловкость, Интеллект",
            ProfSkills = "Четыре навыка из: Акробатика, Атлетика, Восприятие, Выступление, Запугивание, Ловкость рук, Обман, Проницательность, Расследование, Скрытность, Убеждение",
            Equipment = "Рапира, короткий лук и колчан с 20 стрелами, кожаный доспех, два кинжала, воровские инструменты",
            SpellCastingAbility = "Нет",
        },
    ];
}
