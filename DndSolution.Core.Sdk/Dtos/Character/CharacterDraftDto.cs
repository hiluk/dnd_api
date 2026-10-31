using System.ComponentModel.DataAnnotations;
using Core.Sdk.Enums;

namespace Core.Sdk.Dtos.Characters;

/// <summary>
/// Черновик персонажа: всё, из чего по правилам считается лист
/// </summary>
public class CharacterDraftDto
{
    /// <summary>
    /// Раса персонажа
    /// </summary>
    [EnumDataType(typeof(CharacterRaceTypeDto))]
    public CharacterRaceTypeDto CharacterRace { get; init; }

    /// <summary>
    /// Класс персонажа
    /// </summary>
    [EnumDataType(typeof(CharacterClassTypeDto))]
    public CharacterClassTypeDto CharacterClass { get; init; }

    /// <summary>
    /// Способ генерации характеристик
    /// </summary>
    [EnumDataType(typeof(GenerationMethodDto))]
    public GenerationMethodDto GenerationMethod { get; init; }

    /// <summary>
    /// Базовые характеристики (до бонусов расы)
    /// </summary>
    [Required]
    public CharacterStatsDto BaseStats { get; init; } = new();

    /// <summary>
    /// Характеристики, выбранные для бонусов расы «на выбор»
    /// </summary>
    public List<AbilityDto> BonusChoices { get; init; } = [];

    /// <summary>
    /// Подпись результата бросков из <c>POST /abilities/roll</c> (для способа «Броски»)
    /// </summary>
    public string? RollToken { get; init; }
}
