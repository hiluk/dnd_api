using DndSolution.Application.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class TokensRepository : ITokensRepository
{
    private readonly DndContext _context;
    private readonly ILogger<ITokensRepository> _logger;

    public TokensRepository(DndContext context, ILogger<ITokensRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Refresh(int userId, string tokenHash, CancellationToken token)
    {
       var refreshToken =  await _context.Tokens.FirstOrDefaultAsync(t => t.UserId == userId, token);

       if (refreshToken == null) throw new Exception("Токен не найден");

       refreshToken.TokenHash = tokenHash;
       refreshToken.ValidUntil = DateTime.UtcNow.AddDays(7);
       await _context.SaveChangesAsync(token);
    }
}