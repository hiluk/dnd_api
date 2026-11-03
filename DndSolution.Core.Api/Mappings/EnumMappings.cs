using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;

namespace Core.Api.Mappings;

/// <summary>
/// Преобразования списков enum'ов между моделью и DTO.
/// Значения enum'ов в модели и DTO совпадают, поэтому достаточно приведения.
/// </summary>
public static class EnumMappings
{
    public static List<AbilityDto> ToDto(this IEnumerable<Ability> abilities) =>
        abilities.Select(a => (AbilityDto)a).ToList();

    public static List<Ability> ToModel(this IEnumerable<AbilityDto> abilities) =>
        abilities.Select(a => (Ability)a).ToList();

    public static List<SkillDto> ToDto(this IEnumerable<Skill> skills) =>
        skills.Select(s => (SkillDto)s).ToList();

    public static List<Skill> ToModel(this IEnumerable<SkillDto> skills) =>
        skills.Select(s => (Skill)s).ToList();
}
