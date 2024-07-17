using System.Text.Json.Serialization;

namespace DndSolution.Application.Models.Models.Races;

public class Speed
{
    [JsonIgnore]
    public int Id { get; set; }

    public string Type { get; set; }
    
    public int Value { get; set; }

    public Race Race { get; set; }
    public int RaceId { get; set; }
}