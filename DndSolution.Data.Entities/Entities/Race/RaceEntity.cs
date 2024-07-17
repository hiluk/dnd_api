using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

public class RaceEntity
{
    public long raceId { get; init; }
    
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public string Age { get; init; }
    
    public string Size { get; init; }
    
    public string Language { get; init; }
    
    public string Vision { get; init; }
    
    public string Traits { get; init; }
    
    public List<AsiEntity> Asi { get; init; }
    
    public List<SpeedEntity> Speed { get; init; }
}