using Data.Abstractions;
using Data.Repositories.Mappings;
using DndSolution.Application.Models.Models.Backgrounds;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class BackgroundsRepository : IBackgroundsRepository
{
    private readonly DndContext _context;

    public BackgroundsRepository(DndContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<List<Background>> GetAllAsync(CancellationToken token)
    {
        var entities = await _context.Backgrounds
            .AsNoTracking()
            .OrderBy(x => x.Type)
            .ToListAsync(token);

        return entities.Select(e => e.ToModel()).ToList();
    }
}
