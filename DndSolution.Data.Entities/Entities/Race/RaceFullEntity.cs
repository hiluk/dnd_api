using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("asi")]
public class RaceFullEntity
{
    [Key]
    [Column("id")]
    public long Id { get; init; }
    
    [Column("race")]
    public RaceEntity Race { get; init; }
    
    public List<AsiEntity> Asi { get; init; }
    
    public List<SpeedEntity> Speed { get; init; }
}