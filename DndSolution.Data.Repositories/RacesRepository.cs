using Data.Abstractions;
using DndSolution.Application.Models.Models.Races;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class RacesRepository : IRacesRepository
{
    private readonly DndContext _context;

    public RacesRepository(DndContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task SaveRaceAsync(Race race, CancellationToken token)
    {
        // Asi и Speed добавляются вместе с расой как связанные сущности
        await _context.Races.AddAsync(race, token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task<List<Race>> GetAllRaces(CancellationToken token)
    {
        return await _context.Races
            .AsNoTracking()
            .Include(x => x.Asi)
            .Include(x => x.Speed)
            .AsSplitQuery()
            .OrderBy(x => x.Id)
            .ToListAsync(token);
    }
}
