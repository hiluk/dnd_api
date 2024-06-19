using DndSolution.Application.Abstractions;

namespace DndSolution.Application.Models;

public class Character : ICharacter
{
    public string Name { get; init; } = "";
    public int Level { get; init; } = 0;
    public string Class { get; init; } = "Undefined";
}