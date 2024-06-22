using Data.Abstractions;
using Data.Entities;
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

    public async Task SaveCharacterAsync(CharacterEntity character, CancellationToken token)
    {
        await _context.Set<CharacterEntity>().AddAsync(character, token);
        await _context.SaveChangesAsync(token);
    }
}