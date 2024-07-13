using Data.Abstractions;
using Data.Entities.Class;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class ClassesRepository : IClassesRepository
{
    private readonly DndContext _context;

    public ClassesRepository(DndContext context)
    {
        _context = context;
    }

    public async Task SaveClassAsync(CharacterClassEntity entity, CancellationToken token)
    {
        await _context.Set<CharacterClassEntity>().AddAsync(entity, token);
        
        await _context.SaveChangesAsync(token);
    }

    public async Task<List<CharacterClassEntity>> GetAllClassesAsync(CancellationToken token)
    {
        var classes = await _context.Set<CharacterClassEntity>()
            .AsNoTracking()
            .ToListAsync(token);

        return classes;
    }
}