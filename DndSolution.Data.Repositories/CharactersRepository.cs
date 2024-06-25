using Data.Abstractions;
using Data.Entities.Entities;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

public class CharactersRepository : ICharactersRepository
{
    private readonly DndContext _context;
    private readonly ILogger<ICharactersRepository> _logger;

    public CharactersRepository(DndContext context, ILogger<ICharactersRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveCharacterAsync(CharacterFullEntity character, CancellationToken token)
    {
        await _context.Set<CharacterEntity>().AddAsync(character.Character, token);
        await _context.SaveChangesAsync(token);
        
        await _context.Set<CharacterStatsEntity>().AddAsync(character.Stats, token);
        await _context.SaveChangesAsync(token);
    }
}