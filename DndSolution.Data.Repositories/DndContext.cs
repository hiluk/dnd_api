using Data.Entities;
using Data.Entities.Class;
using Data.Entities.Entities;
using DndSolution.Application.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class DndContext : DbContext
{

    public DbSet<Character> Characters { get; set; }
    public DndContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<RaceEntity>().HasKey(x => x.raceId);
        modelBuilder.Entity<CharacterClassEntity>().HasKey(x => x.Id);
        
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
        
        modelBuilder.Entity<Character>().HasKey(x => x.Id);
        modelBuilder.Entity<Character>().Property<string>("Email").IsRequired();
        modelBuilder.Entity<Character>()
            .HasOne(x => x.Stats)
            .WithOne(x => x.Character)
            .HasForeignKey<CharacterStats>(x => x.CharacterId)
            .IsRequired();

        modelBuilder.Entity<CharacterStats>().HasKey(x => x.CharacterId);
        
        modelBuilder.Entity<UserEntity>().HasKey(x => x.Id);
    }
}