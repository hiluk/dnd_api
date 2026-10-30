using System.ComponentModel.DataAnnotations;
using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Персонаж
/// </summary>
public class CharacterDto
{
    /// <summary>
    /// Id персонажа (заполняется сервером)
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Имя персонажа
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(64)]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Уровень персонажа
    /// </summary>
    [Range(1, 20)]
    public byte Level { get; init; } = 1;

    /// <summary>
    /// Класс персонажа
    /// </summary>
    [EnumDataType(typeof(CharacterClassTypeDto))]
    public CharacterClassTypeDto CharacterClass { get; init; }

    /// <summary>
    /// Раса персонажа
    /// </summary>
    [EnumDataType(typeof(CharacterRaceTypeDto))]
    public CharacterRaceTypeDto CharacterRace { get; init; }

    /// <summary>
    /// Характеристики персонажа
    /// </summary>
    [Required]
    public CharacterStatsDto CharacterStats { get; init; } = new();
}
