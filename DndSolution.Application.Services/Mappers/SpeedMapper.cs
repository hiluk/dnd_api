using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Race;

namespace DndSolution.Application.Services.Mappers;

public class SpeedMapper
{
    public static List<SpeedEntity> MapToEntity(List<Speed> speeds)
    {
        List<SpeedEntity> speedEntities = [];
        
        speedEntities.AddRange(speeds.Select(speed => new SpeedEntity
        {
            Type = speed.Type, 
            Value = speed.Value,
        }
        ));

        return speedEntities;
    }

    public static List<Speed> MapToModel(List<SpeedEntity> entities)
    {
        List<Speed> speeds = [];

        foreach (var entity in entities)
        {
            speeds.Add(new Speed
                               {
                                   Type = entity.Type,
                                   Value = entity.Value
                               }
            );
        }

        return speeds;
    }
}