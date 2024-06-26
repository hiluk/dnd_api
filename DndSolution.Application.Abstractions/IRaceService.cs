using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;

namespace DndSolution.Application.Abstractions;

public interface IRaceService
{
    public Task CreateRaceAsync(CharacterFull race, CancellationToken token);
}