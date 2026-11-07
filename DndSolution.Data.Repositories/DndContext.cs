using Data.Entities;
using DndSolution.Neccessary.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories;

public class DndContext : IdentityDbContext<UserEntity, IdentityRole<Guid>, Guid>
{
    public DbSet<CharacterEntity> Characters => Set<CharacterEntity>();
    public DbSet<CharacterSessionEntity> Sessions => Set<CharacterSessionEntity>();
    public DbSet<SessionEventEntity> SessionEvents => Set<SessionEventEntity>();

    public DbSet<RaceEntity> Races => Set<RaceEntity>();

    public DbSet<CharacterClassEntity> Classes => Set<CharacterClassEntity>();

    public DbSet<BackgroundEntity> Backgrounds => Set<BackgroundEntity>();

    public DndContext(DbContextOptions<DndContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Таблицы Identity называются по-старому: asp_net_users и т.д.
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(entity.GetTableName()?.ToSnakeCase());
        }

        // Имена таблиц заданы явно: они не должны зависеть от имён классов сущностей
        modelBuilder.Entity<RaceEntity>(race =>
        {
            race.ToTable("races");
            race.HasKey(x => x.Id);
            race.HasIndex(x => x.Name).IsUnique();
            race.HasMany(x => x.Asi).WithOne(x => x.Race).HasForeignKey(x => x.RaceId);
            race.HasMany(x => x.Speed).WithOne(x => x.Race).HasForeignKey(x => x.RaceId);
        });
        modelBuilder.Entity<AsiEntity>().ToTable("asi").HasKey(x => x.Id);
        modelBuilder.Entity<SpeedEntity>().ToTable("speed").HasKey(x => x.Id);

        modelBuilder.Entity<CharacterClassEntity>(characterClass =>
        {
            characterClass.ToTable("classes");
            characterClass.HasKey(x => x.Id);
            characterClass.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<BackgroundEntity>(background =>
        {
            background.ToTable("backgrounds");
            background.HasKey(x => x.Id);
            background.HasIndex(x => x.Type).IsUnique();
        });

        modelBuilder.Entity<CharacterEntity>(character =>
        {
            character.ToTable("characters");
            character.HasKey(x => x.Id);
            character.Property(x => x.Name).HasMaxLength(64);
            character.HasOne<UserEntity>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            character.HasOne(x => x.Stats)
                .WithOne(x => x.Character)
                .HasForeignKey<CharacterStatsEntity>(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();
            character.OwnsOne(x => x.Avatar, avatar =>
            {
                // Колонки в той же таблице: ключ владельца — это id персонажа.
                // Без явного имени snake_case-конвенция назовёт его character_entity_id
                avatar.ToTable("characters");
                avatar.Property<Guid>("CharacterEntityId").HasColumnName("id");
                avatar.Property(x => x.FileName).HasColumnName("avatar_file_name").HasMaxLength(64);
                avatar.Property(x => x.HasTransparency).HasColumnName("avatar_has_transparency");
                avatar.Property(x => x.Mode).HasColumnName("avatar_mode");
                avatar.Property(x => x.OffsetX).HasColumnName("avatar_offset_x");
                avatar.Property(x => x.OffsetY).HasColumnName("avatar_offset_y");
                avatar.Property(x => x.Scale).HasColumnName("avatar_scale");
            });
            character.Navigation(x => x.Avatar).IsRequired(false);
        });
        modelBuilder.Entity<CharacterStatsEntity>().ToTable("character_stats").HasKey(x => x.CharacterId);

        modelBuilder.Entity<CharacterSessionEntity>(session =>
        {
            session.ToTable("character_sessions");
            session.HasKey(x => x.CharacterId);
            session.HasOne<CharacterEntity>()
                .WithOne()
                .HasForeignKey<CharacterSessionEntity>(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
            session.Property(x => x.Version).IsRowVersion();
            session.Property(x => x.Inventory).HasColumnType("jsonb").HasDefaultValueSql("'{}'::jsonb");
        });

        modelBuilder.Entity<SessionEventEntity>(sessionEvent =>
        {
            sessionEvent.ToTable("session_events");
            sessionEvent.HasKey(x => x.Id);
            sessionEvent.HasOne<CharacterEntity>()
                .WithMany()
                .HasForeignKey(x => x.CharacterId)
                .OnDelete(DeleteBehavior.Cascade);
            sessionEvent.HasIndex(x => new { x.CharacterId, x.ReceivedAt });
            // json, а не jsonb: jsonb переставляет ключи, а поле type
            // полиморфного действия должно идти первым
            sessionEvent.Property(x => x.Action).HasColumnType("json");
            sessionEvent.Property(x => x.Result).HasColumnType("json");
        });
    }
}
