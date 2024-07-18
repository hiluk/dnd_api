using DndSolution.Application.Models;

namespace Data.Repositories;

public interface ITokensRepository
{
    public Task Refresh(RefreshToken refreshToken, CancellationToken token);
}