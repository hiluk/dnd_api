using Core.Sdk.Dtos.Race;
using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Races;

namespace DndSolution.Application.Services;

public class RaceService : IRaceService
{
    private readonly IRacesRepository _repository;

    public RaceService(IRacesRepository repository)
    {
        _repository = repository;
    }

    public async Task SaveRaceAsync(Race race, CancellationToken token)
    {

        await _repository.SaveRaceAsync(race, token);
    }

    public async Task<List<Race>> GetAllRacesAsync(CancellationToken token)
    {
        return await _repository.GetAllRaces(token);

    }
}