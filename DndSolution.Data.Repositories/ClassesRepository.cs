using Data.Abstractions;
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

    public async Task SaveClassAsync(CharacterClass entity, CancellationToken token)
    {
        await _context.Classes.AddAsync(entity, token);
        
        await _context.SaveChangesAsync(token);
    }

    public async Task<List<CharacterClass>> GetAllClassesAsync(CancellationToken token)
    {
        var classes = await _context.Classes
            .AsNoTracking()
            .ToListAsync(token);

        return classes;
    }
}