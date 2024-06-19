using System.ComponentModel.DataAnnotations;
using DndSolution.Application.Abstractions;

namespace DndSolution.Application.Models;

public class Character : ICharacter
{
    [Key]
    public string Name { get; init; } = "";
    
    public int Level { get; init; } = 0;
    
    public string Class { get; init; } = "Undefined";
    
    public IAttributes Attributes { get; set; }
}