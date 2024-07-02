using Core.Sdk.Dtos.Race;
using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Race;
using Mapster;

namespace DndSolution.Application.Services.Mappers;

public class RaceMapper 
{
   public static RaceEntity MapToEntity(Race race)
   {
      return new RaceEntity
      {
         Age = race.Age,
         Description = race.Description,
         Language = race.Language,
         Name = race.Name,
         Size = race.Size,
         Traits = race.Traits,
         Vision = race.Vision,
         Asi = AsiMapper.MapToEntities(race.Asi),
         Speed = SpeedMapper.MapToEntity(race.Speed),
      };
   }
}