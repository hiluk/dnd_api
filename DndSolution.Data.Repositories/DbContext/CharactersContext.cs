

using Data.Entities;
using Microsoft.EntityFrameworkCore;

public class CharactersContext : DbContext
{
    public CharactersContext(DbContextOptions<CharactersContext> options) : base(options)
    {
    }
    
    public DbSet<CharacterEntity> CharacterEntities { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=db;Username=root;Password=root");
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CharacterEntity>().ToTable("Characters");
    }
}
