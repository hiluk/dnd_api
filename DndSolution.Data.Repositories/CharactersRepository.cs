using Data.Abstractions;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

/// <summary>
/// Репозиторий для работы с персонажами
/// </summary>
public class CharactersRepository : ICharactersRepository
{
    private readonly DndContext _context;
    private readonly ILogger<ICharactersRepository> _logger;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="context">Контекст БД</param>
    /// <param name="logger">Логгер</param>
    public CharactersRepository(DndContext context, ILogger<ICharactersRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SaveCharacterAsync(Character character, string email, CancellationToken token)
    {
        await _context.Characters.AddAsync(character, token);
        _context.Characters.Entry(character).Property<string>("Email").CurrentValue = email;
        _context.Characters.Entry(character).Property<DateTime>("CreationTime").CurrentValue = DateTime.UtcNow;
        
        await _context.Set<CharacterStats>().AddAsync(character.Stats, token);
        await _context.SaveChangesAsync(token);
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(string email, CancellationToken token)
    {
        return await _context.Characters
            .Where(c => EF.Property<string>(c, "Email") == email)
            .OrderByDescending(c => EF.Property<DateTime>(c, "CreationTime"))
            .Include(c => c.Stats)
            .AsNoTracking()
            .ToListAsync(token);
        
    }

    /// <inheritdoc />
    public async Task<Character> GetCharacterByIdAsync(string email, Guid id, CancellationToken token)
    {
        return await _context.Characters
            .Where(c => EF.Property<string>(c, "Email") == email && c.Id == id)
            .Include(c => c.Stats)
            .AsNoTracking()
            .FirstAsync(token);
    }

    /// <inheritdoc />
    public async Task DeleteCharacterAsync(string email, string name, CancellationToken token)
    {
        await _context.Characters
            .Where(c => EF.Property<string>(c, "Email") == email && c.Name == name)
            .Include(c => c.Stats)
            .AsNoTracking()
            .ExecuteDeleteAsync(token);
        
        await _context.SaveChangesAsync(token);
    }
}