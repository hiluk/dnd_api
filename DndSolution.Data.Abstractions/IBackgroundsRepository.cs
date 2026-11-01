using DndSolution.Application.Models.Models.Backgrounds;

namespace Data.Abstractions;

public interface IBackgroundsRepository
{
    public Task<List<Background>> GetAllAsync(CancellationToken token);
}
