namespace Core.Sdk.Dtos.Session;

/// <summary>
/// Состояние персонажа в сессии
/// </summary>
public class SessionStateDto
{
    public int CurrentHp { get; init; }

    public int TempHp { get; init; }

    public int HitDiceSpent { get; init; }

    public int DeathSuccesses { get; init; }

    public int DeathFailures { get; init; }

    public bool IsStable { get; init; }

    public bool IsDead { get; init; }

    /// <summary>
    /// Состояния: 1 — ослеплённый … 14 — бессознательный
    /// </summary>
    public List<int> Conditions { get; init; } = [];

    /// <summary>
    /// Истощение, от 0 до 6
    /// </summary>
    public int Exhaustion { get; init; }
}

/// <summary>
/// Сессия персонажа: состояние и данные листа, нужные для показа
/// </summary>
public class SessionDto : SessionStateDto
{
    /// <summary>
    /// Максимум хитов с учётом истощения
    /// </summary>
    public int MaxHp { get; init; }

    /// <summary>
    /// Максимум хитов по листу, без истощения
    /// </summary>
    public int BaseMaxHp { get; init; }

    /// <summary>
    /// Грань кости хитов: 6, 8, 10 или 12
    /// </summary>
    public int HitDie { get; init; }

    /// <summary>
    /// Всего костей хитов (равно уровню)
    /// </summary>
    public int HitDiceTotal { get; init; }

    public int ConModifier { get; init; }
}

/// <summary>
/// Действие игрока. Какие поля нужны, зависит от type:
/// damage (amount, critical), heal (amount), tempHp (amount), setHp (amount),
/// deathSave (roll), stabilize, spendHitDie (roll), longRest,
/// setCondition (condition, active), setExhaustion (level)
/// </summary>
public class SessionActionRequest
{
    /// <summary>
    /// Id действия, выданный клиентом. Повтор с тем же Id не применяется
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Когда действие сделано на устройстве (UTC)
    /// </summary>
    public DateTime CreatedAt { get; init; }

    public string Type { get; init; } = string.Empty;

    public int? Amount { get; init; }

    public bool? Critical { get; init; }

    public int? Roll { get; init; }

    public int? Condition { get; init; }

    public bool? Active { get; init; }

    public int? Level { get; init; }
}

/// <summary>
/// Запись журнала сессии
/// </summary>
public class SessionEventDto
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; init; }

    public string Type { get; init; } = string.Empty;

    public int? Amount { get; init; }

    public bool? Critical { get; init; }

    public int? Roll { get; init; }

    public int? Condition { get; init; }

    public bool? Active { get; init; }

    public int? Level { get; init; }

    public required SessionStateDto Before { get; init; }

    public required SessionStateDto After { get; init; }

    /// <summary>
    /// Что произошло: knockedOut, instantDeath, died, revived, stabilized,
    /// deathSaveSuccess, deathSaveFailure, naturalTwenty, naturalOne, resurrected
    /// </summary>
    public List<string> Outcomes { get; init; } = [];

    /// <summary>
    /// Сколько хитов восстановлено
    /// </summary>
    public int Healed { get; init; }

    /// <summary>
    /// Сколько урона поглотили временные хиты
    /// </summary>
    public int Absorbed { get; init; }
}

/// <summary>
/// Ответ на действие: новое состояние и запись журнала
/// </summary>
public class SessionActionResponse
{
    public required SessionDto Session { get; init; }

    public required SessionEventDto Event { get; init; }
}
