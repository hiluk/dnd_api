using Core.Sdk.Dtos;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Models.Backgrounds;

namespace Core.Api.Mappings;

public static class BackgroundMappings
{
    public static BackgroundDto ToDto(this Background model) => new()
    {
        Type = (BackgroundTypeDto)model.Type,
        Name = model.Name,
        Description = model.Description,
        Skills = model.Skills.ToDto(),
        SkillChoices = model.SkillChoices,
        Tools = model.Tools,
        Languages = model.Languages,
        Equipment = model.Equipment,
        Feature = model.Feature,
    };
}
