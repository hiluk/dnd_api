using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

public class CharacterStatsEntity
{
    public long Id { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор персонажа
    /// </summary>
    public long CharacterId { get; set; }
    
    public CharacterEntity Character { get; init; }
    
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
    /// Выносливость
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
}