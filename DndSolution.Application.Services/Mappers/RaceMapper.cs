using Core.Sdk.Dtos.Race;
using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Race;
using Mapster;

namespace DndSolution.Application.Services.Mappers;

public class RaceMapper 
{
   public static RaceFullEntity MapToEntity(Race race)
   {
      var raceEntity = new RaceEntity
      {
         Age = race.Age,
         Description = race.Description,
         Language = race.Language,
         Name = race.Name,
         Size = race.Size,
         Traits = race.Traits,
         Vision = race.Vision
      };
      
      return new RaceFullEntity
      {
         Race = raceEntity,
         Asi = AsiMapper.MapToEntities(race.Asi, raceEntity.Id),
         Speed = SpeedMapper.MapToEntity(race.Speed, raceEntity.Id),
      };
   }
}