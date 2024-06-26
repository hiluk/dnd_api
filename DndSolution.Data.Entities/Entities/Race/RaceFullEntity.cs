using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

[Table("asi")]
public class RaceFullEntity
{
    
    public RaceEntity Race { get; init; }
    
    public AsiEntity[] Asi { get; init; }
    
    public SpeedEntity[] Speed { get; init; }
}