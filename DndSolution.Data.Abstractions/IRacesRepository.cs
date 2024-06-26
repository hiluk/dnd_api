using Data.Entities.Entities;

namespace Data.Abstractions;

public interface IRacesRepository
{
    public Task SaveRaceAsync(RaceFullEntity race, CancellationToken token);
}