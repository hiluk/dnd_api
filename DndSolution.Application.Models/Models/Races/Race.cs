
namespace DndSolution.Application.Models.Models.Races;

public class Race
{
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public string Age { get; init; }
    
    public string Size { get; init; }
    
    public string Language { get; init; }
    
    public string Vision { get; init; }
    
    public string Traits { get; init; }
    
    public List<Asi> Asi { get; init; }
    
    public List<Speed> Speed { get; init; }
}