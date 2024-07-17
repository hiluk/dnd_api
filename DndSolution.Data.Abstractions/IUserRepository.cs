
using Data.Entities;
using DndSolution.Application.Models;

namespace Data.Abstractions;

public interface IUserRepository
{
    public Task SaveUserAsync(UserEntity user, CancellationToken token);
}