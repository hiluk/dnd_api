namespace DndSolution.Application.Models.Models;

/// <summary>
/// Характеристики персонажа
/// </summary>
public class CharacterStats
{
    /// <summary>
    /// Сила
    /// </summary>
    public int Strength { get; set; }

    /// <summary>
    /// Интеллект
    /// </summary>
    public int Intelligence { get; set; }

    /// <summary>
    /// Мудрость
    /// </summary>
    public int Wisdom { get; set; }

    /// <summary>
    /// Телосложение
    /// </summary>
    public int Constitution { get; set; }

    /// <summary>
    /// Ловкость
    /// </summary>
    public int Dexterity { get; set; }

    /// <summary>
    /// Харизма
    /// </summary>
    public int Charisma { get; set; }

    public Guid CharacterId { get; set; }

    public Character.Character Character { get; set; } = null!;
}
