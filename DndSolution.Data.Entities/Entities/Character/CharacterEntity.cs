﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities.Class;
using Data.Entities.Enums;

namespace Data.Entities.Entities;

[Table("character")]
public class CharacterEntity
{   
    /// <summary>
    /// Идентификатор в бд
    /// </summary>
    [Key]
    [Column("id")]
    public long Id { get; set; }

    // /// <summary>
    // /// Универсальный идентификатор персонажа
    // /// </summary>
    // [Column("character_id")]
    // public string CharacterId { get; init; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    [Column("name")]
    public string Name { get; init; }
    
    /// <summary>
    /// Эмейл пользователя
    /// </summary>
    [Column("email")]
    public string Email { get; init; }
    
    /// <summary>
    /// Уровень персонажа
    /// </summary>
    [Column("level")]
    public byte Level { get; init; }

    /// <summary>
    /// Опыт персонажа
    /// </summary>
    [Column("xp")]
    public long Xp { get; init; }
    
    /// <summary>
    /// Раса персонажа
    /// </summary>
    [Column("race")]
    public CharacterRaceTypeEntity CharacterRace { get; init; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    [Column("class")]
    public CharacterClassTypeEntity CharacterClass { get; init; }

    /// <summary>
    /// Дата создания персонажа
    /// </summary>
    [Column("creation_date")]
    public DateTime DateCreate { get; set; }
    
    public CharacterStatsEntity Stats { get; set; }
}