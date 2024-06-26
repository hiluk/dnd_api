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
    /// Эмейл пользователя
    /// </summary>
    public string Email { get; init; }
    
    /// <summary>
    /// Класс персонажа
    /// </summary>
    public DtoCharacterClass CharacterClass { get; init; }

    /// <summary>
    /// Расса персонажа
    /// </summary>
    public DtoCharacterRace CharacterRace { get; init; }
}