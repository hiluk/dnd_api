using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Data.Repositories;

/// <summary>
/// Применение миграций и синхронизация справочников рас и классов с <see cref="CatalogSeed"/>
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeDbAsync(this IServiceProvider services, CancellationToken token = default)
    {
        await using var scope = services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<DndContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<DndContext>>();

        await context.Database.MigrateAsync(token);

        var (racesAdded, racesUpdated) = await SyncRacesAsync(context, token);
        var (classesAdded, classesUpdated) = await SyncClassesAsync(context, token);
        await context.SaveChangesAsync(token);

        if (racesAdded + racesUpdated + classesAdded + classesUpdated > 0)
        {
            logger.LogInformation(
                "Справочники обновлены. Расы: +{RacesAdded}, изменено {RacesUpdated}. Классы: +{ClassesAdded}, изменено {ClassesUpdated}",
                racesAdded, racesUpdated, classesAdded, classesUpdated);
        }
    }

    private static async Task<(int Added, int Updated)> SyncRacesAsync(DndContext context, CancellationToken token)
    {
        var existing = await context.Races
            .Include(x => x.Asi.OrderBy(a => a.Id))
            .Include(x => x.Speed.OrderBy(s => s.Id))
            .AsSplitQuery()
            .ToDictionaryAsync(x => x.Name, token);

        var (added, updated) = (0, 0);
        foreach (var seed in CatalogSeed.Races)
        {
            if (!existing.TryGetValue(seed.Name, out var race))
            {
                context.Races.Add(seed);
                added++;
                continue;
            }

            var changed = race.Description != seed.Description || race.Age != seed.Age || race.Size != seed.Size
                          || race.Language != seed.Language || race.Vision != seed.Vision || race.Traits != seed.Traits
                          || !race.Asi.Select(a => (a.Stat, a.Value)).SequenceEqual(seed.Asi.Select(a => (a.Stat, a.Value)))
                          || !race.Speed.Select(s => (s.Type, s.Value)).SequenceEqual(seed.Speed.Select(s => (s.Type, s.Value)));
            if (!changed)
            {
                continue;
            }

            // Свойства рас init-only, поэтому значения переписываются через EF
            context.Entry(race).CurrentValues.SetValues(new
            {
                seed.Description, seed.Age, seed.Size, seed.Language, seed.Vision, seed.Traits,
            });
            context.RemoveRange(race.Asi);
            context.RemoveRange(race.Speed);
            race.Asi.AddRange(seed.Asi);
            race.Speed.AddRange(seed.Speed);
            updated++;
        }

        return (added, updated);
    }

    private static async Task<(int Added, int Updated)> SyncClassesAsync(DndContext context, CancellationToken token)
    {
        var classes = await context.Classes.ToListAsync(token);

        foreach (var characterClass in classes)
        {
            if (CatalogSeed.ClassRenames.TryGetValue(characterClass.Name, out var newName)
                && classes.All(x => x.Name != newName))
            {
                characterClass.Name = newName;
            }
        }

        var existing = classes.ToDictionary(x => x.Name);
        var (added, updated) = (0, 0);
        foreach (var seed in CatalogSeed.Classes)
        {
            if (!existing.TryGetValue(seed.Name, out var characterClass))
            {
                context.Classes.Add(seed);
                added++;
                continue;
            }

            var entry = context.Entry(characterClass);
            seed.Id = characterClass.Id;
            entry.CurrentValues.SetValues(seed);
            if (entry.State == EntityState.Modified)
            {
                updated++;
            }
        }

        return (added, updated);
    }
}
