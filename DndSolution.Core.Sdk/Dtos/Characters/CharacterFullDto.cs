namespace Core.Sdk.Dtos.Characters;

public class CharacterFullDto
{
    /// <summary>
    /// Информация о персонаже
    /// </summary>
    public required CharacterDto CharacterInformation { get; init; }
    
    /// <summary>
    /// Информация о очках характеристик персонажа
    /// </summary>
    public required CharacterStatsDto CharacterStats { get; init; }
}