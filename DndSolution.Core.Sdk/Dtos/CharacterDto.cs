using DndSolution.Application.Abstractions;

namespace Core.Sdk.Dtos;

public class CharacterDto : ICharacter
{
    public string Name { get; init; }
    public int Level { get; init; }
    public string Class { get; init; }
    public IAttributes Attributes { get; set; }
}