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
    /// Предыстория. Для превью можно не указывать — тогда навыки не проверяются
    /// и считаются без владения; для создания обязательна
    /// </summary>
    [EnumDataType(typeof(BackgroundTypeDto))]
    public BackgroundTypeDto? Background { get; init; }

    /// <summary>
    /// Навыки, выбранные для своей предыстории
    /// </summary>
    public List<SkillDto> BackgroundSkills { get; init; } = [];

    /// <summary>
    /// Навыки, выбранные от расы (полуэльф)
    /// </summary>
    public List<SkillDto> RaceSkills { get; init; } = [];

    /// <summary>
    /// Навыки, выбранные из списка класса
    /// </summary>
    public List<SkillDto> ClassSkills { get; init; } = [];

    /// <summary>
    /// Навыки с компетентностью (плут)
    /// </summary>
    public List<SkillDto> Expertise { get; init; } = [];

    /// <summary>
    /// Подпись результата бросков из <c>POST /abilities/roll</c> (для способа «Броски»)
    /// </summary>
    public string? RollToken { get; init; }
}
