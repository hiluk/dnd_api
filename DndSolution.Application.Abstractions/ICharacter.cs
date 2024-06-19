namespace DndSolution.Application.Abstractions;

public interface ICharacter
{
    public string Name { get; init; }
    public int Level { get; init; }
    public string Class { get; init; }
    public IAttributes Attributes { get; set; }
}

