using Data.Abstractions;
using Data.Entities.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Data.Repositories.Migrations;

public class RacesRepository : IRacesRepository
{
    private readonly DndContext _context;
    private readonly ILogger<IRacesRepository> _logger;

    public RacesRepository(DndContext context, ILogger<IRacesRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SaveRaceAsync(RaceEntity race, CancellationToken token)
    {
        await _context.Set<RaceEntity>().AddAsync(race, token);

        foreach (var asi in race.Asi)
        {
            await _context.Set<AsiEntity>().AddAsync(asi, token);
        }

        foreach (var speed in race.Speed)
        {
            await _context.Set<SpeedEntity>().AddAsync(speed, token);
        }
        
        await _context.SaveChangesAsync(token); 
    }

    public async Task<List<RaceEntity>> GetAllRaces(CancellationToken token)
    {
        var races = await _context.Set<RaceEntity>()
            .AsNoTracking()
            .Include(x => x.Asi)
            .Include(x => x.Speed)
            .ToListAsync(token);

        return races;
    }
}