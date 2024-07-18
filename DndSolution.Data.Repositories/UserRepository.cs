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

        context.Tokens.Where(t => (DateTime.UtcNow - t.ValidUntil).Milliseconds > 0).ExecuteDelete();
    }

    public async Task SaveUserAsync(User user, CancellationToken token)
    {
        var emails = _context.Set<User>().Where(u => u.Email == user.Email).FirstOrDefaultAsync(token);
        if (emails != null)
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
            .Include(u => u.RefreshTokens)
            .AsNoTracking()
            .FirstAsync(token);
    }
}