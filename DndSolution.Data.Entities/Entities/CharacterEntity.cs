using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Entities.Enums;

namespace Data.Entities.Entities;

[Table("character")]
public class CharacterEntity
{   
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("character_id")]
    public string CharacterId { get; init; }
    
    [Column("name")]
    public string Name { get; init; }
    
    [Column("level")]
    public byte Level { get; init; }

    [Column("xp")]
    public long Xp { get; init; }
    
    [Column("race")]
    public EntityCharacterRace CharacterRace { get; init; }
    
    [Column("class")]
    public EntityCharacterClass CharacterClass { get; init; }

    [Column("creation_date")]
    public DateTime DateCreate { get; set; }

    public CharacterStatsEntity Stats { get; set; }
}