using Core.Sdk.Dtos.Race;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Races;

namespace Core.Api.Mappings;

public static class RaceMappings
{
    public static Race ToModel(this RaceDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description,
        Age = dto.Age,
        Size = dto.Size,
        Language = dto.Language,
        Vision = dto.Vision,
        Traits = dto.Traits,
        Asi = dto.Asi
            .Select(x => new Asi { Stat = x.Stat, Value = x.Value, Ability = (Ability?)x.Ability, Choices = x.Choices })
            .ToList(),
        Speed = dto.Speed.Select(x => new Speed { Type = x.Type, Value = x.Value }).ToList(),
        Skills = dto.Skills.ToModel(),
        SkillChoices = dto.SkillChoices,
    };

    public static RaceDto ToDto(this Race model) => new()
    {
        Name = model.Name,
        Description = model.Description,
        Age = model.Age,
        Size = model.Size,
        Language = model.Language,
        Vision = model.Vision,
        Traits = model.Traits,
        Asi = model.Asi
            .Select(x => new AsiDto { Stat = x.Stat, Value = x.Value, Ability = (AbilityDto?)x.Ability, Choices = x.Choices })
            .ToList(),
        Speed = model.Speed.Select(x => new SpeedDto { Type = x.Type, Value = x.Value }).ToList(),
        Skills = model.Skills.ToDto(),
        SkillChoices = model.SkillChoices,
    };
}
