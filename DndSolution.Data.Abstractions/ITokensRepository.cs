using DndSolution.Application.Models;

namespace Data.Repositories;

public interface ITokensRepository
{
    public Task Refresh(int userId, string tokenHash, CancellationToken token);
}