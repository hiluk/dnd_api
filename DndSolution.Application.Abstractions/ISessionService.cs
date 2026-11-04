using DndSolution.Application.Models.Models.Session;

namespace DndSolution.Application.Abstractions;

/// <summary>
/// Состояние сессии вместе с данными персонажа, нужными для показа
/// </summary>
public sealed record SessionView(SessionState State, SessionContext Context);

/// <summary>
/// Игровая сессия персонажа
/// </summary>
public interface ISessionService
{
    /// <returns>null — персонаж не найден</returns>
    public Task<SessionView?> GetAsync(Guid userId, Guid characterId, CancellationToken token);

    /// <summary>
    /// Применить действие. Повтор действия с тем же Id не применяется:
    /// возвращается уже записанный итог.
    /// </summary>
    /// <returns>null — персонаж не найден</returns>
    /// <exception cref="SessionRulesException">Действие недоступно по правилам</exception>
    public Task<(SessionView View, SessionEvent Event)?> ApplyAsync(
        Guid userId, Guid characterId, Guid actionId, SessionAction action, DateTime createdAt, CancellationToken token);

    /// <returns>null — персонаж не найден</returns>
    public Task<IReadOnlyList<SessionEvent>?> GetEventsAsync(
        Guid userId, Guid characterId, DateTime? before, int limit, CancellationToken token);
}
