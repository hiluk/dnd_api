using DndSolution.Application.Models;

namespace DndSolution.Application.Abstractions;

public interface IUserService
{
    public Task SaveUserAsync(User user, CancellationToken token);
}