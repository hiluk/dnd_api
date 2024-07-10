using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("speed")]
public class SpeedEntity
{
    [Column("id")]
    public long Id { get; init; }
    
    [ForeignKey("race_id")]
    [Column("race_id")]
    public long RaceId { get; init; }
    
    [Column("race")]
    public RaceEntity Race { get; init; }
    
    [Column("type")]
    public string Type { get; set; }
    
    [Column("value")]
    public int Value { get; set; }
}