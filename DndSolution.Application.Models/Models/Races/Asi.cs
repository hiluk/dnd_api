using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Races;

public class Asi
{
    public string Stat { get; init; }
    
    public int Value { get; init; }

    /// <summary>
    /// Характеристика; null — бонус на выбор игрока
    /// </summary>
    public Ability? Ability { get; init; }

    /// <summary>
    /// Сколько разных характеристик выбирается для бонуса на выбор
    /// </summary>
    public int Choices { get; init; } = 1;
}