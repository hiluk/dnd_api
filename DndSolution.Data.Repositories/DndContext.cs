using DndSolution.Application.Models;
using DndSolution.Application.Models.Models;
using DndSolution.Application.Models.Models.Character;
using DndSolution.Application.Models.Models.Classes;
using DndSolution.Application.Models.Models.Races;
using DndSolution.Neccessary.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class DndContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    /// <summary>
    /// Теневое свойство персонажа с Id владельца
    /// </summary>
    public const string CharacterUserIdProperty = "UserId";

    /// <summary>
    /// Теневое свойство персонажа с датой создания
    /// </summary>
    public const string CharacterCreationTimeProperty = "CreationTime";

    public DbSet<Character> Characters => Set<Character>();

    public DbSet<Race> Races => Set<Race>();

    public DbSet<CharacterClass> Classes => Set<CharacterClass>();

    public DndContext(DbContextOptions<DndContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Race>().HasKey(x => x.Id);
        modelBuilder.Entity<Race>().HasIndex(x => x.Name).IsUnique();
        modelBuilder.Entity<CharacterClass>().HasKey(x => x.Id);
        modelBuilder.Entity<CharacterClass>().HasIndex(x => x.Name).IsUnique();

        modelBuilder.Entity<Asi>().HasKey(x => x.Id);
        modelBuilder.Entity<Asi>()
            .HasOne(x => x.Race)
            .WithMany(x => x.Asi)
            .HasForeignKey(x => x.RaceId);

        modelBuilder.Entity<Speed>().HasKey(x => x.Id);
        modelBuilder.Entity<Speed>()
            .HasOne(x => x.Race)
            .WithMany(x => x.Speed)
            .HasForeignKey(x => x.RaceId);

        modelBuilder.Entity<Character>().HasKey(x => x.Id);
        modelBuilder.Entity<Character>().Property(x => x.Name).HasMaxLength(64);
        modelBuilder.Entity<Character>().Property<DateTime>(CharacterCreationTimeProperty).IsRequired();
        modelBuilder.Entity<Character>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(CharacterUserIdProperty)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
        modelBuilder.Entity<Character>()
            .HasOne(x => x.Stats)
            .WithOne(x => x.Character)
            .HasForeignKey<CharacterStats>(x => x.CharacterId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        modelBuilder.Entity<CharacterStats>().HasKey(x => x.CharacterId);

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToSnakeCase());
        }
    }
}
