using DndSolution.Application.Abstractions;

namespace DndSolution.Application.Models;

public class Attributes : IAttributes
{
    public int Strenght { get; set; } = 0;
    public int Intelligence { get; set; } = 0;
    public int Wisdom { get; set; } = 0;
    public int Constitution { get; set; } = 0;
    public int Dexterity { get; set; } = 0;
    public int Charisma { get; set; } = 0;
}