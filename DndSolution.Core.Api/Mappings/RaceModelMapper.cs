using Core.Sdk.Dtos.Race;
using DndSolution.Application.Models.Models.Race;

namespace Core.Api.Mappings;

public static class RaceModelMapper
{
    public static Race MapToModel(RaceDto dto)
    {
        return new Race
        {
            Name = dto.Name,
            Age = dto.Age,
            Asi = AsiMapper.MapToModel(dto.Asi),
            Description = dto.Description,
            Language = dto.Language,
            Size = dto.Size,
            Traits = dto.Traits,
            Vision = dto.Vision,
            Speed = SpeedMapper.MapToModel(dto.Speed),
        };
    }
    
    public static RaceDto MapToDto(Race model)
    {
        return new RaceDto
        {
            Name = model.Name,
            Age = model.Age,
            Asi = AsiMapper.MapToDto(model.Asi),
            Description = model.Description,
            Language = model.Language,
            Size = model.Size,
            Traits = model.Traits,
            Vision = model.Vision,
            Speed = SpeedMapper.MapToDto(model.Speed),
        };
    }
}