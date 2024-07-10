using Core.Sdk.Dtos.Classes;
using Core.Sdk.Dtos.Race;
using Core.Sdk.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Core.Sdk.Dtos.Characters;

public class CharacterDto 
{
    /// <summary>
    /// Имя персонажа
    /// </summary>
    public string Name { get; init; }
    
    /// <summary>
    /// Уровень персонажа
    /// </summary>
    public byte Level { get; init; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    public CharacterClassTypeDto CharacterClass { get; init; }

    /// <summary>
    /// Расса персонажа
    /// </summary>
    public CharacterRaceTypeDto CharacterRace { get; init; }
    
    /// <summary>
    /// Информация о очках характеристик персонажа
    /// </summary>
    public CharacterStatsDto CharacterStats { get; init; }
}