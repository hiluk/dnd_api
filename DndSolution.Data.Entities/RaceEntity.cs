using DndSolution.Application.Models.Enums;

namespace Data.Entities;

/// <summary>
/// Раса (таблица races)
/// </summary>
public class RaceEntity
{
    public int Id { get; set; }

    public CharacterRaceType Type { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Age { get; set; } = string.Empty;

    public string Size { get; set; } = string.Empty;

    public string Language { get; set; } = string.Empty;

    public string Vision { get; set; } = string.Empty;

    public string Traits { get; set; } = string.Empty;

    public List<Skill> Skills { get; set; } = [];

    public int SkillChoices { get; set; }

    public List<AsiEntity> Asi { get; set; } = [];

    public List<SpeedEntity> Speed { get; set; } = [];
}

/// <summary>
/// Увеличение характеристики расой (таблица asi)
/// </summary>
public class AsiEntity
{
    public int Id { get; set; }

    public string Stat { get; set; } = string.Empty;

    public int Value { get; set; }

    public Ability? Ability { get; set; }

    public int Choices { get; set; } = 1;

    public int RaceId { get; set; }

    public RaceEntity Race { get; set; } = null!;
}

/// <summary>
/// Скорость расы (таблица speed)
/// </summary>
public class SpeedEntity
{
    public int Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Value { get; set; }

    public int RaceId { get; set; }

    public RaceEntity Race { get; set; } = null!;
}
