using DndSolution.Application.Models.Enums;

namespace DndSolution.Application.Models.Models.Character;

public class Character
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Уровень персонажа
    /// </summary>
    public byte Level { get; set; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    public CharacterClassType CharacterClass { get; init; }

    /// <summary>
    /// Раса персонажа
    /// </summary>
    public CharacterRaceType CharacterRace { get; set; }
    
    /// <summary>
    /// Характеристики персонажа
    /// </summary>
    public CharacterStats Stats { get; init; }
}