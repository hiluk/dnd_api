using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities.Class;
using Data.Entities.Enums;

namespace Data.Entities.Entities;

public class CharacterEntity
{   
    /// <summary>
    /// Идентификатор в бд
    /// </summary>
    public long Id { get; set; }

    // /// <summary>
    // /// Универсальный идентификатор персонажа
    // /// </summary>
    // [Column("character_id")]
    // public string CharacterId { get; init; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Эмейл пользователя
    /// </summary>
    public string Email { get; init; }
    
    /// <summary>
    /// Уровень персонажа
    /// </summary>
    public byte Level { get; init; }

    /// <summary>
    /// Опыт персонажа
    /// </summary>
    public long Xp { get; init; }
    
    /// <summary>
    /// Раса персонажа
    /// </summary>
    public CharacterRaceTypeEntity CharacterRace { get; init; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    public CharacterClassTypeEntity CharacterClass { get; init; }

    /// <summary>
    /// Дата создания персонажа
    /// </summary>
    public DateTime DateCreate { get; set; }
    
    public CharacterStatsEntity Stats { get; set; }
}