using System.Text.Json.Serialization;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Rules;

namespace DndSolution.Application.Models.Models.Session;

/// <summary>
/// Состояние персонажа в игровой сессии
/// </summary>
public sealed record SessionState
{
    public int CurrentHp { get; init; }

    public int TempHp { get; init; }

    /// <summary>
    /// Потраченные кости хитов
    /// </summary>
    public int HitDiceSpent { get; init; }

    public int DeathSuccesses { get; init; }

    public int DeathFailures { get; init; }

    /// <summary>
    /// Стабилизирован при 0 хитов: спасброски от смерти не нужны
    /// </summary>
    public bool IsStable { get; init; }

    public bool IsDead { get; init; }

    public IReadOnlyList<Condition> Conditions { get; init; } = [];

    /// <summary>
    /// Уровень истощения, от 0 до 6
    /// </summary>
    public int Exhaustion { get; init; }

    /// <summary>
    /// Снаряжение и монеты
    /// </summary>
    public InventoryState Inventory { get; init; } = new();

    /// <summary>
    /// Начальное состояние: полные хиты
    /// </summary>
    public static SessionState Fresh(SessionContext context) => new() { CurrentHp = context.BaseMaxHp };
}

/// <summary>
/// Неизменные для сессии данные персонажа, нужные правилам
/// </summary>
/// <param name="BaseMaxHp">Максимум хитов по листу</param>
/// <param name="HitDie">Грань кости хитов класса: 6, 8, 10 или 12</param>
/// <param name="Level">Уровень: столько же костей хитов</param>
/// <param name="ConModifier">Модификатор Телосложения</param>
public sealed record SessionContext(int BaseMaxHp, int HitDie, int Level, int ConModifier)
{
    /// <summary>
    /// Справочник предметов
    /// </summary>
    public IReadOnlyDictionary<string, ItemDefinition> Catalog { get; init; } = new Dictionary<string, ItemDefinition>();

    /// <summary>
    /// Что нужно для КД, атак и веса; null — снаряжение не считается
    /// </summary>
    public EquipmentProfile? Equipment { get; init; }
}

/// <summary>
/// Защита без доспеха по классу
/// </summary>
public enum UnarmoredDefense
{
    /// <summary>
    /// 10 + Ловкость
    /// </summary>
    None = 0,

    /// <summary>
    /// Варвар: 10 + Ловкость + Телосложение, щит можно
    /// </summary>
    Barbarian = 1,

    /// <summary>
    /// Монах: 10 + Ловкость + Мудрость, без щита
    /// </summary>
    Monk = 2,
}

/// <summary>
/// Данные персонажа для КД, атак и веса
/// </summary>
/// <param name="Scores">Итоговые характеристики</param>
/// <param name="Modifiers">Модификаторы</param>
/// <param name="BaseSpeed">Скорость расы</param>
/// <param name="ArmorProficiencies">light, medium, heavy, shield</param>
/// <param name="WeaponProficiencies">Ключи оружия из справочника, которым владеет персонаж</param>
/// <param name="IgnoresHeavyArmorSpeed">Тяжёлый доспех не снижает скорость (дварф)</param>
public sealed record EquipmentProfile(
    AbilityScores Scores,
    AbilityScores Modifiers,
    int ProficiencyBonus,
    int BaseSpeed,
    UnarmoredDefense Unarmored,
    IReadOnlyList<string> ArmorProficiencies,
    IReadOnlyList<string> WeaponProficiencies,
    bool IgnoresHeavyArmorSpeed);

/// <summary>
/// Действие игрока в сессии. В журнале хранится в JSON с полем type
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(Damage), "damage")]
[JsonDerivedType(typeof(Heal), "heal")]
[JsonDerivedType(typeof(TempHp), "tempHp")]
[JsonDerivedType(typeof(SetHp), "setHp")]
[JsonDerivedType(typeof(DeathSave), "deathSave")]
[JsonDerivedType(typeof(Stabilize), "stabilize")]
[JsonDerivedType(typeof(SpendHitDie), "spendHitDie")]
[JsonDerivedType(typeof(LongRest), "longRest")]
[JsonDerivedType(typeof(SetCondition), "setCondition")]
[JsonDerivedType(typeof(SetExhaustion), "setExhaustion")]
[JsonDerivedType(typeof(AddItem), "addItem")]
[JsonDerivedType(typeof(RemoveItem), "removeItem")]
[JsonDerivedType(typeof(SetEquipped), "setEquipped")]
[JsonDerivedType(typeof(ChangeCurrency), "changeCurrency")]
[JsonDerivedType(typeof(DrinkPotion), "drinkPotion")]
[JsonDerivedType(typeof(UseAmmo), "useAmmo")]
public abstract record SessionAction
{
    public sealed record Damage(int Amount, bool Critical) : SessionAction;

    public sealed record Heal(int Amount) : SessionAction;

    public sealed record TempHp(int Amount) : SessionAction;

    /// <summary>
    /// Поставить хиты вручную («мастер сказал 15»). Хиты выше 0 у погибшего — воскрешение
    /// </summary>
    public sealed record SetHp(int Amount) : SessionAction;

    public sealed record DeathSave(int Roll) : SessionAction;

    public sealed record Stabilize : SessionAction;

    /// <summary>
    /// Потратить кость хитов на коротком отдыхе
    /// </summary>
    public sealed record SpendHitDie(int Roll) : SessionAction;

    public sealed record LongRest : SessionAction;

    public sealed record SetCondition(Condition Condition, bool Active) : SessionAction;

    public sealed record SetExhaustion(int Level) : SessionAction;

    /// <summary>
    /// Добавить предмет: из справочника (<paramref name="Key"/>) или свой (<paramref name="Name"/>).
    /// Предмет из справочника, который уже есть, добавляется к нему количеством
    /// </summary>
    /// <param name="ItemId">Id нового предмета, выданный клиентом</param>
    public sealed record AddItem(
        Guid ItemId, string? Key, int Quantity, string? Name = null, double? Weight = null, string? Note = null)
        : SessionAction;

    public sealed record RemoveItem(Guid ItemId, int Quantity) : SessionAction;

    /// <summary>
    /// Надеть или снять. Новый доспех снимает прежний, новый щит — прежний щит
    /// </summary>
    public sealed record SetEquipped(Guid ItemId, bool Equipped) : SessionAction;

    /// <summary>
    /// Изменить монеты на величины (могут быть отрицательными)
    /// </summary>
    public sealed record ChangeCurrency(int Cp = 0, int Sp = 0, int Ep = 0, int Gp = 0, int Pp = 0) : SessionAction;

    /// <summary>
    /// Выпить зелье: лечение на выпавшее значение, зелий на одно меньше
    /// </summary>
    public sealed record DrinkPotion(Guid ItemId, int Roll) : SessionAction;

    /// <summary>
    /// Потратить боеприпасы
    /// </summary>
    public sealed record UseAmmo(Guid ItemId, int Quantity = 1) : SessionAction;
}

/// <summary>
/// Что произошло в результате действия — для журнала и подсказок
/// </summary>
public enum SessionOutcome
{
    /// <summary>
    /// Хиты упали до 0
    /// </summary>
    KnockedOut = 1,

    /// <summary>
    /// Урон не меньше максимума хитов после 0 — мгновенная смерть
    /// </summary>
    InstantDeath = 2,

    Died = 3,

    /// <summary>
    /// Пришёл в сознание
    /// </summary>
    Revived = 4,

    Stabilized = 5,
    DeathSaveSuccess = 6,
    DeathSaveFailure = 7,

    /// <summary>
    /// 20 на спасброске от смерти: встаёт с 1 хитом
    /// </summary>
    NaturalTwenty = 8,

    /// <summary>
    /// 1 на спасброске от смерти: два провала
    /// </summary>
    NaturalOne = 9,

    /// <summary>
    /// Воскрешён после смерти
    /// </summary>
    Resurrected = 10,
}

/// <summary>
/// Итог действия
/// </summary>
/// <param name="State">Новое состояние</param>
/// <param name="Before">Состояние до действия</param>
/// <param name="Healed">Сколько хитов восстановлено (лечение, кость хитов, отдых)</param>
/// <param name="Absorbed">Сколько урона поглотили временные хиты</param>
public sealed record SessionResult(
    SessionState State,
    SessionState Before,
    IReadOnlyList<SessionOutcome> Outcomes,
    int Healed = 0,
    int Absorbed = 0);

/// <summary>
/// Запись журнала сессии
/// </summary>
/// <param name="Id">Id действия, который выдал клиент: по нему повтор не применяется дважды</param>
public sealed record SessionEvent(
    Guid Id,
    SessionAction Action,
    SessionResult Result,
    DateTime CreatedAt);

/// <summary>
/// Действие нельзя выполнить по правилам
/// </summary>
public sealed class SessionRulesException(string field, string message) : Exception(message)
{
    public string Field { get; } = field;
}
