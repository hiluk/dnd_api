
using Data.Entities.Entities;
using DndSolution.Application.Models.Models.Races;

namespace Data.Abstractions;

public interface IRacesRepository
{
    public Task SaveRaceAsync(RaceEntity race, CancellationToken token);

    public Task<List<RaceEntity>> GetAllRaces(CancellationToken token);
}