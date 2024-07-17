using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities.Entities;

public class AsiEntity
{
    public long Id { get; init; }
    
    public long RaceId { get; init; }
    
    public RaceEntity Race { get; init; }
    
    public string Stat { get; init; }
    
    public int Value { get; init; }
}