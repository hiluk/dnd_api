using Data.Abstractions;
using Data.Repositories.Mappings;
using DndSolution.Application.Models.Models.Classes;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ClassesRepository : IClassesRepository
{
    private readonly DndContext _context;

    public ClassesRepository(DndContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task SaveClassAsync(CharacterClass entity, CancellationToken token)
    {
        await _context.Classes.AddAsync(entity.ToEntity(), token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token)
    {
        var entities = await _context.Classes
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(token);

        return entities.Select(e => e.ToModel()).ToList();
    }
}
