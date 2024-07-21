
using DndSolution.Application.Models;

namespace Data.Abstractions;

public interface IUserRepository
{

    public Task<User> GetByEmail(string email, CancellationToken token);
}