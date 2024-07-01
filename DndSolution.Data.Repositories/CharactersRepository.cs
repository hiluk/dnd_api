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
    public async Task SaveCharacterAsync(CharacterFullEntity character, CancellationToken token)
    {
        await _context.Set<CharacterEntity>().AddAsync(character.Character, token);
        await _context.SaveChangesAsync(token);
        
        await _context.Set<CharacterStatsEntity>().AddAsync(character.Stats, token);
        await _context.SaveChangesAsync(token);
    }
    
    /// <inheritdoc />
    public async Task<IReadOnlyList<CharacterFullEntity>> GetAllUserCharactersAsync(string email, CancellationToken token)
    {
        var characters = _context.Set<CharacterEntity>().Where(x => x.Email == email).AsQueryable();
        var stats = _context.Set<CharacterStatsEntity>().AsQueryable();

        var joined = await characters.Join(stats,
            l => l.CharacterId,
            r => r.CharacterId,
            (l, r) => new
            {
                Character = l,
                Stats = r
            })
            .ToListAsync(token);

        var resultList = new List<CharacterFullEntity>();
        foreach (var item in joined)
        {
            var character = item.Character;
            var stat = item.Stats;

            var resultItem = MapToFullEntity(character, stat);
            resultList.Add(resultItem);
        }
        
        return resultList;
    }

    /// <inheritdoc />
    public async Task<CharacterFullEntity> GetCharacterAsync(string email, string name, CancellationToken token)
    {
        var character = await _context.Set<CharacterEntity>()
            .Where(x => x.Email == email && x.Name == name).FirstAsync(token);

        var stat = await _context.Set<CharacterStatsEntity>()
            .Where(x => x.CharacterId == character.CharacterId).FirstAsync(token);
        
        return MapToFullEntity(character, stat);
    }

    /// <inheritdoc />
    public async Task DeleteCharacterAsync(string email, string name, CancellationToken token)
    {
        await _context.Set<CharacterEntity>()
            .Where(x => x.Email == email && x.Name == name)
            .ExecuteDeleteAsync(token);
        
        await _context.SaveChangesAsync(token);
    }

    private static CharacterFullEntity MapToFullEntity(CharacterEntity character, CharacterStatsEntity stat)
    {
        return new CharacterFullEntity
        {
            Character = character,
            Stats = stat
        };
    }
}