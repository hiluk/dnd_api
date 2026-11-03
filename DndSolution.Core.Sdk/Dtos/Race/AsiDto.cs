using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Race;

public class AsiDto
{
    
    public string Stat { get; init; }
    
    public int Value { get; init; }

    /// <summary>
    /// Характеристика; null — бонус на выбор игрока
    /// </summary>
    public AbilityDto? Ability { get; init; }

    /// <summary>
    /// Сколько разных характеристик выбирается для бонуса на выбор
    /// </summary>
    public int Choices { get; init; } = 1;
}