using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("stats")]
public class CharacterStatsEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
    
    /// <summary>
    /// Уникальный идентификатор персонажа
    /// </summary>
    [Column("character_id")]
    public string CharacterId { get; set; }
    
    /// <summary>
    /// Сила
    /// </summary>
    [Column("strength")]
    public int Strenght { get; set; }
    
    /// <summary>
    /// Интеллект
    /// </summary>
    [Column("intelligence")]
    public int Intelligence { get; set; }
    
    /// <summary>
    /// Мудрость
    /// </summary>
    [Column("wisdom")]
    public int Wisdom { get; set; }
    
    /// <summary>
    /// Выносливость
    /// </summary>
    [Column("constitution")]
    public int Constitution { get; set; }
    
    /// <summary>
    /// Ловкость
    /// </summary>
    [Column("dexterity")]
    public int Dexterity { get; set; }
    
    /// <summary>
    /// Харизма
    /// </summary>
    [Column("charisma")]
    public int Charisma { get; set; }
}