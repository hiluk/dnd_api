using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Races;

namespace DndSolution.Application.Abstractions;

public interface IRaceService
{
    public Task SaveRaceAsync(Race race, CancellationToken token);

    public Task<List<Race>> GetAllRacesAsync(CancellationToken token);
}