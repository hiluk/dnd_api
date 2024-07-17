
using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class DndContext : DbContext
{

    public DbSet<Character> Characters { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<CharacterClass> Classes { get; set; }
    
    public DbSet<User> Users { get; set; }
    
    public DndContext(DbContextOptions options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Race>().HasKey(x => x.Id);
        modelBuilder.Entity<CharacterClass>().HasKey(x => x.Id);
        
        modelBuilder.Entity<Asi>().HasKey(x => x.Id);
        modelBuilder.Entity<Asi>()
                    .HasOne<Race>(x => x.Race)
                    .WithMany(x => x.Asi)
                    .HasForeignKey(x => x.RaceId);
        
        modelBuilder.Entity<Speed>().HasKey(x => x.Id);
        modelBuilder.Entity<Speed>()
            .HasOne<Race>(x => x.Race)
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
        
        modelBuilder.Entity<User>().HasKey(x => x.Id);
    }
}