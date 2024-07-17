
using DndSolution.Application.Models;

namespace Data.Abstractions;

public interface IUserRepository
{
    public Task SaveUserAsync(User user, CancellationToken token);

    public Task<User> GetByEmail(string email, CancellationToken token);
}