using System.Text.Json.Serialization;

namespace DndSolution.Application.Models.Models.Races;

public class Asi
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Stat { get; init; }
    
    public int Value { get; init; }

    public int RaceId { get; set; }
    public Race Race { get; set; }
}