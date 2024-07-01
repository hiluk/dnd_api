using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Race;

namespace DndSolution.Application.Services.Mappers;

public class AsiMapper
{
    public static List<AsiEntity> MapToEntities(List<Asi> asis, long id)
    {
        List<AsiEntity> asiEntities = [];

        foreach (var asi in asis)
        {
            asiEntities.Add(new AsiEntity
            {
                Value = asi.Value,
                Stat = asi.Stat,
                Id = id
            }
            );
        }

        return asiEntities;
    }
}