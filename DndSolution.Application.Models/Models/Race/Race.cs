﻿namespace DndSolution.Application.Models.Models.Race;

public class Race
{
    public string Id { get; init; }
    
    public string Name { get; init; }
    
    public string Description { get; init; }
    
    public string Age { get; init; }
    
    public string Size { get; init; }
    
    public string Language { get; init; }
    
    public string Vision { get; init; }
    
    public string Traits { get; init; }
    
    public Asi[] Asi { get; init; }
    
    public Speed[] Speed { get; init; }
}