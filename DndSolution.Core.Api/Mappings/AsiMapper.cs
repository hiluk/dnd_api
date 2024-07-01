using Core.Sdk.Dtos.Race;
using DndSolution.Application.Models.Models.Race;

namespace Core.Api.Mappings;

public class AsiMapper
{
    public static List<Asi> MapToModel(List<AsiDto> dtos)
    {
        List<Asi> models = [];

        foreach (var asiDto in dtos)
        {
            models.Add(new Asi
            {
                Stat = asiDto.Stat,
                Value = asiDto.Value,
            });
        }

        return models;
    }
}