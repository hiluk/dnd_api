﻿namespace Core.Sdk.Dtos.Race;

public class RaceDto
{
    
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public string Age { get; init; }
    
    public string Size { get; init; }
    
    public string Language { get; init; }
    
    public string Vision { get; init; }
    
    public string Traits { get; init; }
    
    public List<AsiDto> Asi { get; init; }
    
    public List<SpeedDto> Speed { get; init; }
}