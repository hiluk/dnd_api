using Data.Entities.Entities;

namespace Data.Abstractions;

public interface IRacesRepository
{
    public Task SaveRaceAsync(RaceEntity race, CancellationToken token);

    public Task<List<RaceEntity>> GetAllRaces(CancellationToken token);
}