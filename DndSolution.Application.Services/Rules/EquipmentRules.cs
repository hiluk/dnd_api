using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Session;

namespace DndSolution.Application.Services.Rules;

/// <summary>
/// Атака оружием в руках
/// </summary>
/// <param name="Damage">Урон с модификатором: «1к8 + 3»</param>
/// <param name="Versatile">Урон двумя руками для универсального оружия</param>
/// <param name="AmmoItemId">Боеприпасы из снаряжения для этого оружия</param>
public sealed record WeaponAttack(
    Guid ItemId,
    string Name,
    int AttackBonus,
    string Damage,
    string DamageType,
    string? Versatile,
    IReadOnlyList<int>? Range,
    bool Proficient,
    Guid? AmmoItemId,
    int? AmmoCount);

/// <summary>
/// Что даёт снаряжение: КД, скорость, вес, атаки и предупреждения
/// </summary>
/// <param name="ArmorSource">Откуда КД: «Кольчуга, щит», «Без доспеха»</param>
/// <param name="Weight">Вес снаряжения и монет, фунтов</param>
/// <param name="Capacity">Грузоподъёмность: Сила × 15</param>
public sealed record EquipmentSummary(
    int ArmorClass,
    string ArmorSource,
    int Speed,
    bool StealthDisadvantage,
    double Weight,
    int Capacity,
    bool Encumbered,
    IReadOnlyList<string> Warnings,
    IReadOnlyList<WeaponAttack> Attacks);

/// <summary>
/// КД от доспеха и щита, скорость, вес и атаки оружием по «Книге игрока».
/// Повторено в приложении и проверяется общим набором примеров.
/// </summary>
public static class EquipmentRules
{
    /// <summary>
    /// 50 монет весят 1 фунт
    /// </summary>
    public const int CoinsPerPound = 50;

    public static EquipmentSummary Evaluate(SessionContext context, InventoryState inventory)
    {
        var profile = context.Equipment
            ?? throw new InvalidOperationException("Для снаряжения нужен профиль персонажа");
        var mods = profile.Modifiers;
        var equipped = inventory.Items
            .Where(i => i.Equipped && i.Key is not null && context.Catalog.ContainsKey(i.Key))
            .Select(i => (Item: i, Definition: context.Catalog[i.Key!]))
            .ToList();
        var armor = equipped.FirstOrDefault(e => e.Definition.Armor is not null);
        var shield = equipped.FirstOrDefault(e => e.Definition.Shield is not null);
        var warnings = new List<string>();

        // КД
        int armorClass;
        var sources = new List<string>();
        if (armor.Definition?.Armor is { } info)
        {
            var dex = info.DexCap is { } cap ? Math.Min(mods.Dexterity, cap) : mods.Dexterity;
            armorClass = info.BaseAc + dex;
            sources.Add(armor.Definition.Name);
            if (!profile.ArmorProficiencies.Contains(info.Type))
            {
                warnings.Add($"Нет владения доспехом «{armor.Definition.Name}»: помеха на проверки, спасброски "
                             + "и атаки Силы и Ловкости, заклинания недоступны");
            }
        }
        else
        {
            armorClass = profile.Unarmored switch
            {
                UnarmoredDefense.Barbarian => 10 + mods.Dexterity + mods.Constitution,
                UnarmoredDefense.Monk when shield.Definition is null => 10 + mods.Dexterity + mods.Wisdom,
                _ => 10 + mods.Dexterity,
            };
            sources.Add(profile.Unarmored switch
            {
                UnarmoredDefense.Barbarian => "Защита без доспеха",
                UnarmoredDefense.Monk when shield.Definition is null => "Защита без доспеха",
                _ => "Без доспеха",
            });
        }

        if (shield.Definition?.Shield is { } shieldInfo)
        {
            armorClass += shieldInfo.Bonus;
            sources.Add(shield.Definition.Name.ToLowerInvariant());
            if (!profile.ArmorProficiencies.Contains("shield"))
            {
                warnings.Add("Нет владения щитом: помеха на проверки, спасброски и атаки Силы и Ловкости");
            }
        }

        // Скорость: тяжёлый доспех без нужной Силы — минус 10 футов (дварфу не мешает)
        var speed = profile.BaseSpeed;
        if (armor.Definition?.Armor is { Strength: { } strength } && profile.Scores.Strength < strength)
        {
            if (!profile.IgnoresHeavyArmorSpeed)
            {
                speed = Math.Max(0, speed - 10);
                warnings.Add($"Сила {profile.Scores.Strength} меньше {strength}: скорость −10 фт.");
            }
        }

        // Вес
        var weight = inventory.Items.Sum(i => i.Quantity * UnitWeight(context, i))
                     + (double)inventory.Currency.Count / CoinsPerPound;
        weight = Math.Round(weight, 2, MidpointRounding.AwayFromZero);
        var capacity = profile.Scores.Strength * 15;
        if (weight > capacity)
        {
            warnings.Add($"Перегруз: {Format(weight)} из {capacity} фнт.");
        }

        // Атаки
        var attacks = equipped
            .Where(e => e.Definition.Weapon is not null)
            .Select(e => Attack(context, profile, inventory, e.Item, e.Definition))
            .ToList();

        foreach (var (item, definition) in equipped)
        {
            if (shield.Definition is not null && definition.Weapon?.Has("twoHanded") == true)
            {
                warnings.Add($"«{definition.Name}» — двуручное оружие, а в руке щит");
            }
        }

        return new EquipmentSummary(
            armorClass,
            string.Join(", ", sources),
            speed,
            armor.Definition?.Armor?.StealthDisadvantage == true,
            weight,
            capacity,
            weight > capacity,
            warnings,
            attacks);
    }

    /// <summary>
    /// Вес одной штуки: из справочника или свой
    /// </summary>
    public static double UnitWeight(SessionContext context, InventoryItem item) =>
        item.Key is { } key && context.Catalog.TryGetValue(key, out var definition)
            ? definition.UnitWeight
            : item.Weight ?? 0;

    private static WeaponAttack Attack(
        SessionContext context, EquipmentProfile profile, InventoryState inventory,
        InventoryItem item, ItemDefinition definition)
    {
        var weapon = definition.Weapon!;
        var mods = profile.Modifiers;

        // Дальнобойное — Ловкость, рукопашное — Сила, фехтовальное — лучшая из двух
        var ability = weapon.Has("finesse")
            ? Math.Max(mods.Strength, mods.Dexterity)
            : weapon.Ranged ? mods.Dexterity : mods.Strength;
        var proficient = profile.WeaponProficiencies.Contains(definition.Key);
        var bonus = ability + (proficient ? profile.ProficiencyBonus : 0);

        // Боеприпасы: первые подходящие из снаряжения
        InventoryItem? ammo = null;
        if (weapon.Ammo is { } kind)
        {
            ammo = inventory.Items.FirstOrDefault(i =>
                i.Key is { } key && context.Catalog.TryGetValue(key, out var a) && a.Ammunition?.Kind == kind);
        }

        return new WeaponAttack(
            item.Id,
            definition.Name,
            bonus,
            Damage(weapon.Damage, ability),
            weapon.DamageType,
            weapon.Versatile is { } versatile ? Damage(versatile, ability) : null,
            weapon.Range,
            proficient,
            ammo?.Id,
            weapon.Ammo is null ? null : ammo?.Quantity ?? 0);
    }

    /// <summary>
    /// «1к8 + 3», «2к6 − 1», «1 + 3»; у сети урона нет — «—»
    /// </summary>
    public static string Damage(string dice, int modifier)
    {
        if (dice == "0")
        {
            return "—";
        }

        var text = dice.Replace('d', 'к');
        return modifier == 0 ? text : $"{text} {(modifier > 0 ? "+" : "−")} {Math.Abs(modifier)}";
    }

    private static string Format(double value) =>
        value.ToString(value % 1 == 0 ? "0" : "0.##", System.Globalization.CultureInfo.InvariantCulture);
}
