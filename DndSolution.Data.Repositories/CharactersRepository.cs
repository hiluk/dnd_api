using Data.Abstractions;
using DndSolution.Application.Models.Models;
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
    private ICharactersRepository _charactersRepositoryImplementation;

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
        
        await _context.Set<CharacterStats>().AddAsync(character.Stats, token);
        await _context.SaveChangesAsync(token);
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(string email, CancellationToken token)
    {
        return await _context.Characters
            .Where(c => EF.Property<string>(c, "Email") == email)
            .Include(c => c.Stats)
            .AsNoTracking()
            .ToListAsync(token);
    }

    /// <inheritdoc />
    public async Task<Character> GetCharacterAsync(string email, string name, CancellationToken token)
    {
        return await _context.Characters
            .Where(c => EF.Property<string>(c, "Email") == email)
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