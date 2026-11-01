using Data.Abstractions;
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
        return await _context.Backgrounds
            .AsNoTracking()
            .OrderBy(x => x.Type)
            .ToListAsync(token);
    }
}
