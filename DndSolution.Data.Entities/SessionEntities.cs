using DndSolution.Application.Models.Enums;

namespace Data.Entities;

/// <summary>
/// Состояние персонажа в игровой сессии (таблица character_sessions)
/// </summary>
public class CharacterSessionEntity
{
    public Guid CharacterId { get; set; }

    public int CurrentHp { get; set; }

    public int TempHp { get; set; }

    public int HitDiceSpent { get; set; }

    public int DeathSuccesses { get; set; }

    public int DeathFailures { get; set; }

    public bool IsStable { get; set; }

    public bool IsDead { get; set; }

    public List<Condition> Conditions { get; set; } = [];

    public int Exhaustion { get; set; }

    /// <summary>
    /// Версия строки (системный столбец xmin): два одновременных действия не затрут друг друга
    /// </summary>
    public uint Version { get; set; }
}

/// <summary>
/// Запись журнала сессии (таблица session_events)
/// </summary>
public class SessionEventEntity
{
    /// <summary>
    /// Id действия, выданный клиентом: повтор того же действия не применяется
    /// </summary>
    public Guid Id { get; set; }

    public Guid CharacterId { get; set; }

    /// <summary>
    /// Когда действие сделано на устройстве
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Когда сервер его принял
    /// </summary>
    public DateTime ReceivedAt { get; set; }

    /// <summary>
    /// Действие в JSON
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Итог действия в JSON: состояние до и после, исходы
    /// </summary>
    public string Result { get; set; } = string.Empty;
}
