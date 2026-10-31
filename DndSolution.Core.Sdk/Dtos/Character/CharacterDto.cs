using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Персонаж
/// </summary>
public class CharacterDto
{
    /// <summary>
    /// Id персонажа
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Имя персонажа
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Уровень персонажа
    /// </summary>
    public byte Level { get; init; }

    /// <summary>
    /// Класс персонажа
    /// </summary>
    public CharacterClassTypeDto CharacterClass { get; init; }

    /// <summary>
    /// Раса персонажа
    /// </summary>
    public CharacterRaceTypeDto CharacterRace { get; init; }

    /// <summary>
    /// Способ генерации характеристик
    /// </summary>
    public GenerationMethodDto GenerationMethod { get; init; }

    /// <summary>
    /// Базовые характеристики (до бонусов расы)
    /// </summary>
    public required CharacterStatsDto BaseStats { get; init; }

    /// <summary>
    /// Характеристики, выбранные для бонусов расы «на выбор»
    /// </summary>
    public required List<AbilityDto> BonusChoices { get; init; }

    /// <summary>
    /// Итоговые и производные значения
    /// </summary>
    public required CharacterSheetDto Sheet { get; init; }
}
