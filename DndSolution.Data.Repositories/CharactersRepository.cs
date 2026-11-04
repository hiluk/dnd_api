using Data.Abstractions;
using Data.Repositories.Mappings;
using DndSolution.Application.Models.Models.Avatars;
using DndSolution.Application.Models.Models.Character;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

/// <summary>
/// Репозиторий для работы с персонажами
/// </summary>
public class CharactersRepository : ICharactersRepository
{
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
        var entity = character.ToEntity(userId, DateTime.UtcNow);
        await _context.Characters.AddAsync(entity, token);
        await _context.SaveChangesAsync(token);

        character.Id = entity.Id;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Character>> GetAllUserCharactersAsync(Guid userId, CancellationToken token)
    {
        var entities = await _context.Characters
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.CreationTime)
            .Include(c => c.Stats)
            .ToListAsync(token);

        return entities.Select(e => e.ToModel()).ToList();
    }

    /// <inheritdoc />
    public async Task<Character?> GetCharacterByIdAsync(Guid userId, Guid id, CancellationToken token)
    {
        var entity = await _context.Characters
            .AsNoTracking()
            .Where(c => c.UserId == userId && c.Id == id)
            .Include(c => c.Stats)
            .FirstOrDefaultAsync(token);

        return entity?.ToModel();
    }

    /// <inheritdoc />
    public async Task<bool> DeleteCharacterAsync(Guid userId, Guid id, CancellationToken token)
    {
        // Характеристики удаляются каскадно на уровне БД
        var deleted = await _context.Characters
            .Where(c => c.UserId == userId && c.Id == id)
            .ExecuteDeleteAsync(token);

        return deleted > 0;
    }

    public async Task<bool> SetAvatarAsync(Guid userId, Guid id, CharacterAvatar? avatar, CancellationToken token)
    {
        var entity = await _context.Characters
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == id, token);
        if (entity is null)
        {
            return false;
        }

        entity.Avatar = avatar?.ToEntity();
        await _context.SaveChangesAsync(token);

        return true;
    }
}
