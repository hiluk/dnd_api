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
        modelBuilder.Entity<CharacterEntity>().HasKey(x => x.Id);
        modelBuilder.Entity<CharacterStatsEntity>().HasKey(x => x.Id);
        
        modelBuilder.Entity<UserEntity>().HasKey(x => x.Id);
    }
}