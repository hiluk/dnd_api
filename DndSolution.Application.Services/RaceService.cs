using Core.Sdk.Dtos.Race;
using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Models.Race;
using DndSolution.Application.Services.Mappers;

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
        var entity = RaceMapper.MapToEntity(race);

        await _repository.SaveRaceAsync(entity, token);
    }
}