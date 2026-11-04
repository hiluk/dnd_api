using System.Text.Json;
using Data.Abstractions;
using Data.Entities;
using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Session;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Data.Repositories;

public class SessionRepository : ISessionRepository
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly DndContext _context;

    public SessionRepository(DndContext context)
    {
        _context = context;
    }

    public async Task<SessionState?> GetStateAsync(Guid characterId, CancellationToken token)
    {
        var entity = await _context.Sessions.AsNoTracking()
            .FirstOrDefaultAsync(s => s.CharacterId == characterId, token);

        return entity is null ? null : ToModel(entity);
    }

    public async Task<SessionEvent?> GetEventAsync(Guid characterId, Guid eventId, CancellationToken token)
    {
        var entity = await _context.SessionEvents.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId && e.CharacterId == characterId, token);

        return entity is null ? null : ToModel(entity);
    }

    public async Task<bool> SaveAsync(
        Guid characterId, SessionState? expected, SessionEvent sessionEvent, CancellationToken token)
    {
        var entity = await _context.Sessions.FirstOrDefaultAsync(s => s.CharacterId == characterId, token);
        if (entity is null != expected is null || (entity is not null && !Same(ToModel(entity), expected!)))
        {
            return false;
        }

        if (entity is null)
        {
            entity = new CharacterSessionEntity { CharacterId = characterId };
            _context.Sessions.Add(entity);
        }

        Apply(entity, sessionEvent.Result.State);
        _context.SessionEvents.Add(new SessionEventEntity
        {
            Id = sessionEvent.Id,
            CharacterId = characterId,
            CreatedAt = sessionEvent.CreatedAt,
            ReceivedAt = DateTime.UtcNow,
            Action = JsonSerializer.Serialize(sessionEvent.Action, Json),
            Result = JsonSerializer.Serialize(sessionEvent.Result, Json),
        });

        try
        {
            await _context.SaveChangesAsync(token);
            return true;
        }
        catch (DbUpdateException e) when (e is DbUpdateConcurrencyException
                                           || e.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            // Кто-то успел раньше: состояние изменилось или действие уже записано
            _context.ChangeTracker.Clear();
            return false;
        }
    }

    public async Task<IReadOnlyList<SessionEvent>> GetEventsAsync(
        Guid characterId, DateTime? before, int limit, CancellationToken token)
    {
        var query = _context.SessionEvents.AsNoTracking().Where(e => e.CharacterId == characterId);
        if (before is not null)
        {
            query = query.Where(e => e.ReceivedAt < before);
        }

        var entities = await query.OrderByDescending(e => e.ReceivedAt).Take(limit).ToListAsync(token);
        return entities.Select(ToModel).ToList();
    }

    /// <summary>
    /// Состояния равны: сравниваем JSON, потому что в состоянии есть списки
    /// </summary>
    private static bool Same(SessionState a, SessionState b) =>
        JsonSerializer.Serialize(a, Json) == JsonSerializer.Serialize(b, Json);

    private static SessionState ToModel(CharacterSessionEntity entity) => new()
    {
        CurrentHp = entity.CurrentHp,
        TempHp = entity.TempHp,
        HitDiceSpent = entity.HitDiceSpent,
        DeathSuccesses = entity.DeathSuccesses,
        DeathFailures = entity.DeathFailures,
        IsStable = entity.IsStable,
        IsDead = entity.IsDead,
        Conditions = entity.Conditions.ToList(),
        Exhaustion = entity.Exhaustion,
        Inventory = JsonSerializer.Deserialize<InventoryState>(entity.Inventory, Json) ?? new InventoryState(),
    };

    private static void Apply(CharacterSessionEntity entity, SessionState state)
    {
        entity.CurrentHp = state.CurrentHp;
        entity.TempHp = state.TempHp;
        entity.HitDiceSpent = state.HitDiceSpent;
        entity.DeathSuccesses = state.DeathSuccesses;
        entity.DeathFailures = state.DeathFailures;
        entity.IsStable = state.IsStable;
        entity.IsDead = state.IsDead;
        entity.Conditions = state.Conditions.ToList();
        entity.Exhaustion = state.Exhaustion;
        entity.Inventory = JsonSerializer.Serialize(state.Inventory, Json);
    }

    private static SessionEvent ToModel(SessionEventEntity entity) => new(
        entity.Id,
        JsonSerializer.Deserialize<SessionAction>(entity.Action, Json)!,
        JsonSerializer.Deserialize<SessionResult>(entity.Result, Json)!,
        DateTime.SpecifyKind(entity.CreatedAt, DateTimeKind.Utc));
}
