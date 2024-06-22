using Core.Sdk.Enums;

namespace Core.Sdk.Dtos;

public class CharacterDto 
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
    public DtoCharacterClass CharacterClass { get; init; }

    /// <summary>
    /// Расса персонажа
    /// </summary>
    public DtoCharacterRace CharacterRace { get; init; }
}