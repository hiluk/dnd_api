
using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Race;
using Mapster;

namespace DndSolution.Application.Services.Mappers;

[Mapper]
public interface IRacesMapper
{
    RaceEntity MapTo(Race race);
    Race MapTo(RaceFullEntity raceEntity);
}