using Core.Sdk.Dtos.Race;
using DndSolution.Application.Models.Models.Races;

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
    
    public static List<AsiDto> MapToDto(List<Asi> models)
    {
        List<AsiDto> dtos = [];

        foreach (var asi in models)
        {
            dtos.Add(new AsiDto
            {
                Stat = asi.Stat,
                Value = asi.Value,
            });
        }

        return dtos;
    }
}