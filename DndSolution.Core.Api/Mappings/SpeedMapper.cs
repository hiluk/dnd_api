using Core.Sdk.Dtos.Race;
using DndSolution.Application.Models.Models.Race;

namespace Core.Api.Mappings;

public class SpeedMapper
{
    public static List<Speed> MapToModel(List<SpeedDto> dtos)
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
    
    public static List<SpeedDto> MapToDto(List<Speed> models)
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
}