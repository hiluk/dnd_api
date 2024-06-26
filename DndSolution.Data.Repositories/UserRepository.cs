using Data.Abstractions;
using Data.Entities;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DndContext _context;
    private readonly ILogger<IUserRepository> _logger;

    public UserRepository(DndContext context, ILogger<IUserRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveUserAsync(UserEntity user, CancellationToken token)
    {
        var emails = _context.Set<UserEntity>().Select(x => x.Email).ToList();
        if (emails.Contains(user.Email))
        {
            _logger.LogError("Пользователь с таким эмэйлом уже есть");
            throw new ArgumentException();
        }
            
        await _context.Set<UserEntity>().AddAsync(user, token);
        await _context.SaveChangesAsync(token);
    }
}