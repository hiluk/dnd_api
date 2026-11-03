using Data.Abstractions;
using Data.Repositories.Mappings;
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
        await _context.Races.AddAsync(race.ToEntity(), token);
        await _context.SaveChangesAsync(token);
    }

    /// <inheritdoc />
    public async Task<List<Race>> GetAllRaces(CancellationToken token)
    {
        var entities = await _context.Races
            .AsNoTracking()
            .Include(x => x.Asi.OrderBy(a => a.Id))
            .Include(x => x.Speed.OrderBy(s => s.Id))
            .AsSplitQuery()
            .OrderBy(x => x.Id)
            .ToListAsync(token);

        return entities.Select(e => e.ToModel()).ToList();
    }
}
