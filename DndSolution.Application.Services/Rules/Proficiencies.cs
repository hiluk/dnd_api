using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Session;

namespace DndSolution.Application.Services.Rules;

/// <summary>
/// Владение доспехами и оружием по классу и расе («Книга игрока»)
/// </summary>
public static class Proficiencies
{
    private static readonly string[] AllArmor = ["light", "medium", "heavy", "shield"];
    private static readonly string[] MediumArmor = ["light", "medium", "shield"];
    private static readonly string[] LightArmor = ["light"];

    private static readonly string[] RogueWeapons = ["hand-crossbow", "longsword", "rapier", "shortsword"];
    private static readonly string[] DruidWeapons =
        ["club", "dagger", "dart", "javelin", "mace", "quarterstaff", "scimitar", "sickle", "sling", "spear"];
    private static readonly string[] MageWeapons = ["dagger", "dart", "sling", "quarterstaff", "light-crossbow"];

    public static IReadOnlyList<string> Armor(CharacterClassType type) => type switch
    {
        CharacterClassType.Fighter or CharacterClassType.Paladin => AllArmor,
        CharacterClassType.Barbarian or CharacterClassType.Ranger or CharacterClassType.Cleric
            or CharacterClassType.Druid => MediumArmor,
        CharacterClassType.Rogue or CharacterClassType.Bard or CharacterClassType.Warlock => LightArmor,
        _ => [],
    };

    /// <summary>
    /// Ключи оружия из справочника, которым владеет персонаж
    /// </summary>
    public static IReadOnlyList<string> Weapons(
        CharacterClassType type, CharacterRaceType race, IReadOnlyDictionary<string, ItemDefinition> catalog)
    {
        IEnumerable<string> Category(string category) =>
            catalog.Values.Where(i => i.Weapon?.Category == category).Select(i => i.Key);

        var weapons = type switch
        {
            CharacterClassType.Fighter or CharacterClassType.Barbarian or CharacterClassType.Ranger
                or CharacterClassType.Paladin => Category("simple").Concat(Category("martial")),
            CharacterClassType.Rogue or CharacterClassType.Bard => Category("simple").Concat(RogueWeapons),
            CharacterClassType.Cleric or CharacterClassType.Warlock => Category("simple"),
            CharacterClassType.Monk => Category("simple").Append("shortsword"),
            CharacterClassType.Druid => DruidWeapons,
            CharacterClassType.Sorcerer or CharacterClassType.Wizard => MageWeapons,
            _ => [],
        };

        IEnumerable<string> racial = race switch
        {
            CharacterRaceType.Dwarf => ["battleaxe", "handaxe", "light-hammer", "warhammer"],
            CharacterRaceType.Elf => ["longsword", "shortsword", "shortbow", "longbow"],
            _ => [],
        };

        return weapons.Concat(racial).Distinct().Order().ToList();
    }

    public static UnarmoredDefense Unarmored(CharacterClassType type) => type switch
    {
        CharacterClassType.Barbarian => UnarmoredDefense.Barbarian,
        CharacterClassType.Monk => UnarmoredDefense.Monk,
        _ => UnarmoredDefense.None,
    };

    /// <summary>
    /// Дварфу тяжёлый доспех не снижает скорость
    /// </summary>
    public static bool IgnoresHeavyArmorSpeed(CharacterRaceType race) => race == CharacterRaceType.Dwarf;
}
