using Data.Abstractions;
using Data.Entities.Entities;
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
    public async Task SaveCharacterAsync(CharacterEntity character, CancellationToken token)
    {
        await _context.Set<CharacterEntity>().AddAsync(character, token);
        
        await _context.Set<CharacterStatsEntity>().AddAsync(character.Stats, token);
        await _context.SaveChangesAsync(token);
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<CharacterEntity>> GetAllUserCharactersAsync(string email, CancellationToken token)
    {
        var characters = await _context.Set<CharacterEntity>()
            .Where(x => x.Email == email)
            .Include(x => x.Stats)
            .ToListAsync(token);

        return characters;
    }

    /// <inheritdoc />
    public async Task<CharacterEntity> GetCharacterAsync(string email, string name, CancellationToken token)
    {
        var character = await _context.Set<CharacterEntity>()
            .Where(x => x.Email == email && x.Name == name)
            .Include(x => x.Stats)
            .FirstAsync(token);
        
        
        return character;
    }

    /// <inheritdoc />
    public async Task DeleteCharacterAsync(string email, string name, CancellationToken token)
    {
        await _context.Set<CharacterEntity>()
            .Where(x => x.Email == email && x.Name == name)
            .ExecuteDeleteAsync(token);
        
        await _context.SaveChangesAsync(token);
    }
}