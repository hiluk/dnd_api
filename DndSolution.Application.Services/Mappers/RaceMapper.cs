using Core.Sdk.Dtos.Race;
using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Races;
using Mapster;

namespace DndSolution.Application.Services.Mappers;

public class RaceMapper 
{
   public static List<AsiEntity> MapAsiToEntities(List<Asi> asis)
   {
      List<AsiEntity> asiEntities = [];

      foreach (var asi in asis)
      {
         asiEntities.Add(new AsiEntity
            {
               Value = asi.Value,
               Stat = asi.Stat,
            }
         );
      }

      return asiEntities;
   }
    
   public static List<Asi> MapAsiToModels(List<AsiEntity> entities)
   {
      List<Asi> asis = [];

      foreach (var asi in entities)
      {
         asis.Add(new Asi
            {
               Value = asi.Value,
               Stat = asi.Stat,
            }
         );
      }

      return asis;
   }
   
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
         Asi = MapAsiToEntities(race.Asi),
         Speed = MapSpeedToEntity(race.Speed),
      };
   }

   public static Race MapToModel(RaceEntity entity)
   {
      return new Race
      {
         Age = entity.Age,
         Description = entity.Description,
         Language = entity.Language,
         Name = entity.Name,
         Size = entity.Size,
         Traits = entity.Traits,
         Vision = entity.Vision,
         Asi = MapAsiToModels(entity.Asi),
         Speed = MapSpeedToModels(entity.Speed),
      };
   }
   
   public static List<SpeedEntity> MapSpeedToEntity(List<Speed> speeds)
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

   public static List<Speed> MapSpeedToModels(List<SpeedEntity> entities)
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