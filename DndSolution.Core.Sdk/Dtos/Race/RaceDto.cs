namespace Core.Sdk.Dtos.Race;

public class RaceDto
{
    public string Id { get; init; }
    
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public string Age { get; init; }
    
    public string Size { get; init; }
    
    public string Language { get; init; }
    
    public string Vision { get; init; }
    
    public string Traits { get; init; }
    
    public AsiDto[] Asi { get; init; }
    
    public SpeedDto[] Speed { get; init; }
}