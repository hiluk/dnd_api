using DndSolution.Application.Abstractions;

namespace Core.Sdk;

public class CharactersDto : ICharacter
{
    public string Name { get; init; }
    public int Level { get; init; }
    public string Class { get; init; }
}