using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("asi")]
public class AsiEntity
{
    [Column("id")]
    public long Id { get; init; }
    
    [ForeignKey("race_id")]
    [Column("race_id")]
    public long RaceId { get; init; }
    
    [Column("race")]
    public RaceEntity Race { get; init; }
    
    [Column("stat")]
    public string Stat { get; init; }
    
    [Column("value")]
    public int Value { get; init; }
}