using Data.Abstractions;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;

namespace DndSolution.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task SaveUserAsync(User user, CancellationToken token)
    {
        await _repository.SaveUserAsync(user, token);
    }
}