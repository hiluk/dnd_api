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
}