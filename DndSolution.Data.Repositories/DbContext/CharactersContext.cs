

using Data.Entities;
using Microsoft.EntityFrameworkCore;

public class CharactersDbContext : DbContext
{
    public CharactersDbContext(DbContextOptions<CharactersDbContext> options) : base(options)
    {
    }
    
    public DbSet<CharacterEntity> CharacterEntities { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost:5432;Database=dnddb1;Username=postgres;Password=dndapipass");
    } 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CharacterEntity>().ToTable("Characters");
    }
}
