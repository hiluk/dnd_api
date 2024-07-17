using Data.Abstractions;
using DndSolution.Application.Models;
using Microsoft.EntityFrameworkCore;
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

    public async Task SaveUserAsync(User user, CancellationToken token)
    {
        var emails = _context.Set<User>().Select(x => x.Email).ToList();
        if (emails.Contains(user.Email))
        {
            _logger.LogError("Пользователь с таким эмэйлом уже есть");
            throw new ArgumentException();
        }
            
        await _context.Set<User>().AddAsync(user, token);
        await _context.SaveChangesAsync(token);
    }

    public async Task<User> GetByEmail(string email, CancellationToken token)
    {
        return await _context.Users
            .Where(u => u.Email == email)
            .AsNoTracking()
            .FirstAsync(token);
    }
}