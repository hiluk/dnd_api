using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

public class SpeedEntity
{
    public long Id { get; init; }
    
    public long RaceId { get; init; }
    
    public RaceEntity Race { get; init; }
    
    public string Type { get; set; }
    
    public int Value { get; set; }
}