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

    public async Task<User> GetByEmail(string email, CancellationToken token)
    {
        return await _context.Users.Where(x => x.Email == email).FirstAsync(token);
    }
}