namespace Core.Sdk.Dtos;

public class CharacterSaveRequest
{
    /// <summary>
    /// Информация о персонаже
    /// </summary>
    public CharacterDto CharacterInformation { get; set; }
    
    /// <summary>
    /// Информация о очках характеристик персонажа
    /// </summary>
    public CharacterStatsDto CharacterStats { get; set; }
}