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
        
        context.Tokens.Where(t => (DateTime.UtcNow - t.ValidUntil).Milliseconds > 0).ExecuteDeleteAsync();
    }

    public async Task Refresh(RefreshToken refreshToken, CancellationToken token)
    { 
        await _context.Tokens.AddAsync(refreshToken, token);
        await _context.SaveChangesAsync(token);
    }
}