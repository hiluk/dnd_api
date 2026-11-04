namespace Core.Sdk.Dtos.Session;

/// <summary>
/// Состояние персонажа в сессии
/// </summary>
public class SessionStateDto
{
    public int CurrentHp { get; init; }

    public int TempHp { get; init; }

    public int HitDiceSpent { get; init; }

    public int DeathSuccesses { get; init; }

    public int DeathFailures { get; init; }

    public bool IsStable { get; init; }

    public bool IsDead { get; init; }

    /// <summary>
    /// Состояния: 1 — ослеплённый … 14 — бессознательный
    /// </summary>
    public List<int> Conditions { get; init; } = [];

    /// <summary>
    /// Истощение, от 0 до 6
    /// </summary>
    public int Exhaustion { get; init; }

    public InventoryDto Inventory { get; init; } = new();
}

/// <summary>
/// Монеты: медные, серебряные, электрум, золотые, платиновые
/// </summary>
public class CurrencyDto
{
    public int Cp { get; init; }

    public int Sp { get; init; }

    public int Ep { get; init; }

    public int Gp { get; init; }

    public int Pp { get; init; }
}

public class InventoryItemDto
{
    public Guid Id { get; init; }

    /// <summary>
    /// Ключ предмета из справочника; null — свой предмет
    /// </summary>
    public string? Key { get; init; }

    public string? Name { get; init; }

    /// <summary>
    /// Вес одной штуки своего предмета, фунтов
    /// </summary>
    public double? Weight { get; init; }

    public string? Note { get; init; }

    public int Quantity { get; init; }

    public bool Equipped { get; init; }
}

public class InventoryDto
{
    public CurrencyDto Currency { get; init; } = new();

    public List<InventoryItemDto> Items { get; init; } = [];
}

/// <summary>
/// Атака оружием в руках
/// </summary>
public class WeaponAttackDto
{
    public Guid ItemId { get; init; }

    public string Name { get; init; } = string.Empty;

    public int AttackBonus { get; init; }

    /// <summary>
    /// «1к8 + 3»
    /// </summary>
    public string Damage { get; init; } = string.Empty;

    public string DamageType { get; init; } = string.Empty;

    /// <summary>
    /// Урон двумя руками, если оружие универсальное
    /// </summary>
    public string? Versatile { get; init; }

    public List<int>? Range { get; init; }

    public bool Proficient { get; init; }

    public Guid? AmmoItemId { get; init; }

    public int? AmmoCount { get; init; }
}

/// <summary>
/// Что даёт снаряжение
/// </summary>
public class EquipmentSummaryDto
{
    public int ArmorClass { get; init; }

    /// <summary>
    /// Откуда КД: «Кольчуга, щит»
    /// </summary>
    public string ArmorSource { get; init; } = string.Empty;

    /// <summary>
    /// Скорость с учётом доспеха (без истощения)
    /// </summary>
    public int Speed { get; init; }

    public bool StealthDisadvantage { get; init; }

    public double Weight { get; init; }

    public int Capacity { get; init; }

    public bool Encumbered { get; init; }

    public List<string> Warnings { get; init; } = [];

    public List<WeaponAttackDto> Attacks { get; init; } = [];
}

/// <summary>
/// Данные персонажа для расчёта снаряжения на устройстве без связи
/// </summary>
public class EquipmentProfileDto
{
    /// <summary>
    /// Итоговые характеристики: strength, dexterity, …
    /// </summary>
    public Dictionary<string, int> Scores { get; init; } = [];

    public Dictionary<string, int> Modifiers { get; init; } = [];

    public int ProficiencyBonus { get; init; }

    public int BaseSpeed { get; init; }

    /// <summary>
    /// 0 — нет, 1 — варвар, 2 — монах
    /// </summary>
    public int Unarmored { get; init; }

    public List<string> ArmorProficiencies { get; init; } = [];

    public List<string> WeaponProficiencies { get; init; } = [];

    public bool IgnoresHeavyArmorSpeed { get; init; }
}

/// <summary>
/// Сессия персонажа: состояние и данные листа, нужные для показа
/// </summary>
public class SessionDto : SessionStateDto
{
    /// <summary>
    /// Максимум хитов с учётом истощения
    /// </summary>
    public int MaxHp { get; init; }

    /// <summary>
    /// Максимум хитов по листу, без истощения
    /// </summary>
    public int BaseMaxHp { get; init; }

    /// <summary>
    /// Грань кости хитов: 6, 8, 10 или 12
    /// </summary>
    public int HitDie { get; init; }

    /// <summary>
    /// Всего костей хитов (равно уровню)
    /// </summary>
    public int HitDiceTotal { get; init; }

    public int ConModifier { get; init; }

    public required EquipmentSummaryDto Equipment { get; init; }

    public required EquipmentProfileDto Profile { get; init; }
}

/// <summary>
/// Действие игрока. Какие поля нужны, зависит от type:
/// damage (amount, critical), heal (amount), tempHp (amount), setHp (amount),
/// deathSave (roll), stabilize, spendHitDie (roll), longRest,
/// setCondition (condition, active), setExhaustion (level),
/// addItem (itemId, key или name, quantity, weight, note), removeItem (itemId, quantity),
/// setEquipped (itemId, equipped), changeCurrency (cp, sp, ep, gp, pp),
/// drinkPotion (itemId, roll), useAmmo (itemId, quantity)
/// </summary>
public class SessionActionRequest
{
    /// <summary>
    /// Id действия, выданный клиентом. Повтор с тем же Id не применяется
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Когда действие сделано на устройстве (UTC)
    /// </summary>
    public DateTime CreatedAt { get; init; }

    public string Type { get; init; } = string.Empty;

    public int? Amount { get; init; }

    public bool? Critical { get; init; }

    public int? Roll { get; init; }

    public int? Condition { get; init; }

    public bool? Active { get; init; }

    public int? Level { get; init; }

    public Guid? ItemId { get; init; }

    public string? Key { get; init; }

    public string? Name { get; init; }

    public double? Weight { get; init; }

    public string? Note { get; init; }

    public int? Quantity { get; init; }

    public bool? Equipped { get; init; }

    public int? Cp { get; init; }

    public int? Sp { get; init; }

    public int? Ep { get; init; }

    public int? Gp { get; init; }

    public int? Pp { get; init; }
}

/// <summary>
/// Запись журнала сессии
/// </summary>
public class SessionEventDto
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string Type { get; init; } = string.Empty;

    public int? Amount { get; init; }

    public bool? Critical { get; init; }

    public int? Roll { get; init; }

    public int? Condition { get; init; }

    public bool? Active { get; init; }

    public int? Level { get; init; }

    public Guid? ItemId { get; init; }

    public string? Key { get; init; }

    public string? Name { get; init; }

    public double? Weight { get; init; }

    public string? Note { get; init; }

    public int? Quantity { get; init; }

    public bool? Equipped { get; init; }

    public int? Cp { get; init; }

    public int? Sp { get; init; }

    public int? Ep { get; init; }

    public int? Gp { get; init; }

    public int? Pp { get; init; }

    public required SessionStateDto Before { get; init; }

    public required SessionStateDto After { get; init; }

    /// <summary>
    /// Что произошло: knockedOut, instantDeath, died, revived, stabilized,
    /// deathSaveSuccess, deathSaveFailure, naturalTwenty, naturalOne, resurrected
    /// </summary>
    public List<string> Outcomes { get; init; } = [];

    /// <summary>
    /// Сколько хитов восстановлено
    /// </summary>
    public int Healed { get; init; }

    /// <summary>
    /// Сколько урона поглотили временные хиты
    /// </summary>
    public int Absorbed { get; init; }
}

/// <summary>
/// Ответ на действие: новое состояние и запись журнала
/// </summary>
public class SessionActionResponse
{
    public required SessionDto Session { get; init; }

    public required SessionEventDto Event { get; init; }
}
