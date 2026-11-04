using System.Text.Json;
using System.Text.Json.Serialization;

namespace DndSolution.Application.Models.Models.Items;

/// <summary>
/// Предмет из справочника (SRD 5.1). Вес — в фунтах, цена — в медных монетах за <see cref="Bundle"/> штук
/// </summary>
public sealed record ItemDefinition
{
    public required string Key { get; init; }

    public required string Name { get; init; }

    public required ItemCategory Category { get; init; }

    public double Weight { get; init; }

    public int Cost { get; init; }

    /// <summary>
    /// Сколько штук продаётся за цену и весит <see cref="Weight"/> (стрелы — по 20)
    /// </summary>
    public int Bundle { get; init; } = 1;

    public WeaponInfo? Weapon { get; init; }

    public ArmorInfo? Armor { get; init; }

    public ShieldInfo? Shield { get; init; }

    public AmmunitionInfo? Ammunition { get; init; }

    public PotionInfo? Potion { get; init; }

    /// <summary>
    /// Содержимое набора
    /// </summary>
    public IReadOnlyList<PackEntry>? Pack { get; init; }

    /// <summary>
    /// Вес одной штуки
    /// </summary>
    public double UnitWeight => Weight / Bundle;
}

[JsonConverter(typeof(CamelCaseEnumConverter<ItemCategory>))]
public enum ItemCategory
{
    Weapon,
    Armor,
    Shield,
    Ammunition,
    Potion,
    Gear,
    Tool,
    Pack,
}

/// <summary>
/// Enum в JSON строкой в camelCase: «weapon», «ammunition»
/// </summary>
public sealed class CamelCaseEnumConverter<T>() : JsonStringEnumConverter<T>(JsonNamingPolicy.CamelCase)
    where T : struct, Enum;

public sealed record WeaponInfo
{
    /// <summary>
    /// simple или martial
    /// </summary>
    public required string Category { get; init; }

    public bool Ranged { get; init; }

    /// <summary>
    /// Кости урона: «1d8», «2d6», «1» (духовая трубка), «0» (сеть)
    /// </summary>
    public required string Damage { get; init; }

    public string DamageType { get; init; } = string.Empty;

    /// <summary>
    /// Урон двумя руками для универсального оружия
    /// </summary>
    public string? Versatile { get; init; }

    /// <summary>
    /// light, finesse, heavy, twoHanded, thrown, reach, loading, ammunition, versatile, special
    /// </summary>
    public IReadOnlyList<string> Properties { get; init; } = [];

    /// <summary>
    /// Дистанция: обычная и максимальная, в футах
    /// </summary>
    public IReadOnlyList<int>? Range { get; init; }

    /// <summary>
    /// Вид боеприпаса: arrow, bolt, needle, bullet
    /// </summary>
    public string? Ammo { get; init; }

    public bool Has(string property) => Properties.Contains(property);
}

public sealed record ArmorInfo
{
    /// <summary>
    /// light, medium или heavy
    /// </summary>
    public required string Type { get; init; }

    public int BaseAc { get; init; }

    /// <summary>
    /// Предел бонуса Ловкости: null — без предела, 0 — Ловкость не добавляется
    /// </summary>
    public int? DexCap { get; init; }

    /// <summary>
    /// Нужная Сила: без неё скорость меньше на 10 футов
    /// </summary>
    public int? Strength { get; init; }

    public bool StealthDisadvantage { get; init; }
}

public sealed record ShieldInfo(int Bonus);

public sealed record AmmunitionInfo(string Kind);

public sealed record PotionInfo(string Heal);

public sealed record PackEntry(string Key, int Quantity);

/// <summary>
/// Монеты: медные, серебряные, электрум, золотые, платиновые
/// </summary>
public sealed record Currency(int Cp = 0, int Sp = 0, int Ep = 0, int Gp = 0, int Pp = 0)
{
    public int Count => Cp + Sp + Ep + Gp + Pp;

    /// <summary>
    /// Всё в медных монетах
    /// </summary>
    public int InCopper => Cp + Sp * 10 + Ep * 50 + Gp * 100 + Pp * 1000;
}

/// <summary>
/// Предмет у персонажа: из справочника (<see cref="Key"/>) или свой
/// (<see cref="Name"/>, <see cref="Weight"/>)
/// </summary>
public sealed record InventoryItem
{
    /// <summary>
    /// Id выдаёт клиент при добавлении: следующие действия без связи ссылаются на него
    /// </summary>
    public Guid Id { get; init; }

    public string? Key { get; init; }

    public string? Name { get; init; }

    /// <summary>
    /// Вес одной штуки своего предмета
    /// </summary>
    public double? Weight { get; init; }

    public string? Note { get; init; }

    public int Quantity { get; init; } = 1;

    /// <summary>
    /// Надето или в руках
    /// </summary>
    public bool Equipped { get; init; }
}

public sealed record InventoryState
{
    public Currency Currency { get; init; } = new();

    public IReadOnlyList<InventoryItem> Items { get; init; } = [];
}
