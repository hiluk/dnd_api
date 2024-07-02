﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DndSolution.Application.Models.Models.Race;

namespace Data.Entities.Entities;

[Table("race")]
public class RaceEntity
{
    [Key]
    [Column("id")]
    public long raceId { get; init; }
    
    [Column("name")]
    public string Name { get; init; }
    
    [Column("desc")]
    public string Description { get; init; }
    
    [Column("age")]
    public string Age { get; init; }
    
    [Column("size")]
    public string Size { get; init; }
    
    [Column("language")]
    public string Language { get; init; }
    
    [Column("vision")]
    public string Vision { get; init; }
    
    [Column("traits")]
    public string Traits { get; init; }
    
    public List<AsiEntity> Asi { get; init; }
    
    public List<SpeedEntity> Speed { get; init; }
}