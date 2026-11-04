using DndSolution.Application.Models.Models.Session;

namespace Data.Abstractions;

/// <summary>
/// Состояние игровой сессии и её журнал
/// </summary>
public interface ISessionRepository
{
    /// <returns>null — сессия ещё не начиналась</returns>
    public Task<SessionState?> GetStateAsync(Guid characterId, CancellationToken token);

    /// <returns>null — такого действия ещё не было</returns>
    public Task<SessionEvent?> GetEventAsync(Guid characterId, Guid eventId, CancellationToken token);

    /// <summary>
    /// Сохранить новое состояние и запись журнала одной транзакцией
    /// </summary>
    /// <param name="expected">Состояние, от которого считали; null — сессии не было</param>
    /// <returns>false — состояние успели изменить или действие уже записано: нужно перечитать</returns>
    public Task<bool> SaveAsync(
        Guid characterId, SessionState? expected, SessionEvent sessionEvent, CancellationToken token);

    /// <summary>
    /// Журнал от новых к старым
    /// </summary>
    public Task<IReadOnlyList<SessionEvent>> GetEventsAsync(
        Guid characterId, DateTime? before, int limit, CancellationToken token);
}
