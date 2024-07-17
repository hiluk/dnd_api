

using DndSolution.Application.Models.Models.Races;

namespace Data.Abstractions;

public interface IRacesRepository
{
    public Task SaveRaceAsync(Race race, CancellationToken token);

    public Task<List<Race>> GetAllRaces(CancellationToken token);
}