using System.ComponentModel.DataAnnotations;

namespace Core.Sdk.Dtos;

/// <summary>
/// Характеристики персонажа
/// </summary>
public class CharacterStatsDto
{
    /// <summary>
    /// Сила
    /// </summary>
    [Range(0, 30)]
    public int Strength { get; set; }

    /// <summary>
    /// Интеллект
    /// </summary>
    [Range(0, 30)]
    public int Intelligence { get; set; }

    /// <summary>
    /// Мудрость
    /// </summary>
    [Range(0, 30)]
    public int Wisdom { get; set; }

    /// <summary>
    /// Телосложение
    /// </summary>
    [Range(0, 30)]
    public int Constitution { get; set; }

    /// <summary>
    /// Ловкость
    /// </summary>
    [Range(0, 30)]
    public int Dexterity { get; set; }

    /// <summary>
    /// Харизма
    /// </summary>
    [Range(0, 30)]
    public int Charisma { get; set; }
}
