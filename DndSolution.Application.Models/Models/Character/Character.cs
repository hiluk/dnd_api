

using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;

namespace DndSolution.Application.Models.Models;

public class Character
{
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