using Data.Entities;
using Data.Entities.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class DndContext : DbContext
{
    public DndContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RaceEntity>().HasKey(x => x.raceId);
        
        modelBuilder.Entity<AsiEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<AsiEntity>()
                    .HasOne<RaceEntity>(x => x.Race)
                    .WithMany(x => x.Asi)
                    .HasForeignKey(x => x.RaceId);
        
        modelBuilder.Entity<SpeedEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<SpeedEntity>()
            .HasOne<RaceEntity>(x => x.Race)
            .WithMany(x => x.Speed)
            .HasForeignKey(x => x.RaceId);
        
        modelBuilder.Entity<CharacterEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<CharacterStatsEntity>().HasKey(x => x.Id);
        
        modelBuilder.Entity<UserEntity>().HasKey(x => x.Id);
    }
}