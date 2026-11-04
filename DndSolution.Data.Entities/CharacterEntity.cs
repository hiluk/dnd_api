using DndSolution.Application.Models.Enums;

namespace Data.Entities;

/// <summary>
/// Персонаж (таблица characters)
/// </summary>
public class CharacterEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Владелец
    /// </summary>
    public Guid UserId { get; set; }

    public DateTime CreationTime { get; set; }

    public string Name { get; set; } = string.Empty;

    public byte Level { get; set; }

    public CharacterClassType CharacterClass { get; set; }

    public CharacterRaceType CharacterRace { get; set; }

    public GenerationMethod GenerationMethod { get; set; }

    public List<Ability> BonusChoices { get; set; } = [];

    public BackgroundType Background { get; set; }

    public List<Skill> BackgroundSkills { get; set; } = [];

    public List<Skill> RaceSkills { get; set; } = [];

    public List<Skill> ClassSkills { get; set; } = [];

    public List<Skill> Expertise { get; set; } = [];

    /// <summary>
    /// Базовые характеристики
    /// </summary>
    public CharacterStatsEntity Stats { get; set; } = null!;

    /// <summary>
    /// Своя картинка; null — стандартная. Хранится в колонках avatar_* таблицы characters
    /// </summary>
    public CharacterAvatarEntity? Avatar { get; set; }
}

/// <summary>
/// Своя картинка персонажа (часть таблицы characters)
/// </summary>
public class CharacterAvatarEntity
{
    public string FileName { get; set; } = string.Empty;

    public bool HasTransparency { get; set; }

    public AvatarMode Mode { get; set; }

    public double OffsetX { get; set; }

    public double OffsetY { get; set; }

    public double Scale { get; set; }
}

/// <summary>
/// Базовые характеристики персонажа (таблица character_stats)
/// </summary>
public class CharacterStatsEntity
{
    public Guid CharacterId { get; set; }

    public int Strength { get; set; }

    public int Intelligence { get; set; }

    public int Wisdom { get; set; }

    public int Constitution { get; set; }

    public int Dexterity { get; set; }

    public int Charisma { get; set; }

    public CharacterEntity Character { get; set; } = null!;
}
