using Core.Sdk.Dtos.Race;
using Core.Sdk.Enums;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Races;

namespace Core.Api.Mappings;

public static class RaceModelMapper
{
    
    
    
    public static List<Asi> MapAsiToModel(List<AsiDto> dtos)
    {
        List<Asi> models = [];

        foreach (var asiDto in dtos)
        {
            models.Add(new Asi
            {
                Stat = asiDto.Stat,
                Value = asiDto.Value,
                Ability = (Ability?)asiDto.Ability,
                Choices = asiDto.Choices,
            });
        }

        return models;
    }
    public static List<Speed> MapSpeedToModel(List<SpeedDto> dtos)
    {
        List<Speed> models = [];

        foreach (var speedDto in dtos)
        {
            models.Add(new Speed
                {
                    Type = speedDto.Type,
                    Value = speedDto.Value,
                }
            );
        }

        return models;
    }

    public static List<SpeedDto> MapSpeedToDto(List<Speed> models)
    {
        List<SpeedDto> dtos = [];

        foreach (var speed in models)
        {
            dtos.Add(new SpeedDto
                {
                    Type = speed.Type,
                    Value = speed.Value,
                }
            );
        }

        return dtos;
    }

    public static List<AsiDto> MapAsiToDto(List<Asi> models)
    {
        List<AsiDto> dtos = [];

        foreach (var asi in models)
        {
            dtos.Add(new AsiDto
            {
                Stat = asi.Stat,
                Value = asi.Value,
                Ability = (AbilityDto?)asi.Ability,
                Choices = asi.Choices,
            });
        }

        return dtos;
    }
    
    
    public static Race MapToModel(RaceDto dto)
    {
        return new Race
        {
            Name = dto.Name,
            Age = dto.Age,
            Asi = MapAsiToModel(dto.Asi),
            Description = dto.Description,
            Language = dto.Language,
            Size = dto.Size,
            Traits = dto.Traits,
            Vision = dto.Vision,
            Speed = MapSpeedToModel(dto.Speed),
            Skills = dto.Skills.Select(x => (Skill)x).ToList(),
            SkillChoices = dto.SkillChoices,
        };
    }
    
    public static RaceDto MapToDto(Race model)
    {
        return new RaceDto
        {
            Name = model.Name,
            Age = model.Age,
            Asi = MapAsiToDto(model.Asi),
            Description = model.Description,
            Language = model.Language,
            Size = model.Size,
            Traits = model.Traits,
            Vision = model.Vision,
            Speed = MapSpeedToDto(model.Speed),
            Skills = model.Skills.Select(x => (SkillDto)x).ToList(),
            SkillChoices = model.SkillChoices,
        };
    }
}