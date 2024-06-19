using DndSolution.Application.Abstractions;

namespace Core.Sdk;

public class AttributesDto : IAttributes
{
    public int Strenght { get; set; }
    public int Intelligence { get; set; }
    public int Wisdom { get; set; }
    public int Constitution { get; set; }
    public int Dexterity { get; set; }
    public int Charisma { get; set; }
}