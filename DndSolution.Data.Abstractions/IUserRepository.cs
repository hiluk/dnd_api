using Data.Entities;

namespace Data.Abstractions;

public interface IUserRepository
{
    public Task SaveUserAsync(UserEntity user, CancellationToken token);
}