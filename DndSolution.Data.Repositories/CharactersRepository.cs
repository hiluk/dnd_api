using Data.Abstractions;
using DndSolution.Application.Models.Models.Character;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <summary>
/// Репозиторий для работы с персонажами
/// </summary>
public class CharactersRepository : ICharactersRepository
{
    private const string UserIdProperty = DndContext.CharacterUserIdProperty;
    private const string CreationTimeProperty = DndContext.CharacterCreationTimeProperty;

    private readonly DndContext _context;

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="context">Контекст БД</param>
    public CharactersRepository(DndContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task SaveCharacterAsync(Character character, Guid userId, CancellationToken token)
    {
        var entry = await _context.Characters.AddAsync(character, token);
        entry.Property<Guid>(UserIdProperty).CurrentValue = userId;
        entry.Property<DateTime>(CreationTimeProperty).CurrentValue = DateTime.UtcNow;

        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(Guid userId, CancellationToken token)
    {
        return await _context.Characters
            .AsNoTracking()
            .Where(c => EF.Property<Guid>(c, UserIdProperty) == userId)
            .OrderByDescending(c => EF.Property<DateTime>(c, CreationTimeProperty))
            .Include(c => c.Stats)
            .ToListAsync(token);
    }

    /// <inheritdoc />
    public async Task<Character?> GetCharacterByIdAsync(Guid userId, Guid id, CancellationToken token)
    {
        return await _context.Characters
            .AsNoTracking()
            .Where(c => EF.Property<Guid>(c, UserIdProperty) == userId && c.Id == id)
            .Include(c => c.Stats)
            .FirstOrDefaultAsync(token);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCharacterAsync(Guid userId, Guid id, CancellationToken token)
    {
        // Характеристики удаляются каскадно на уровне БД
        var deleted = await _context.Characters
            .Where(c => EF.Property<Guid>(c, UserIdProperty) == userId && c.Id == id)
            .ExecuteDeleteAsync(token);

        return deleted > 0;
    }
}
