using System.Text.Json;
using Core.Sdk.Dtos.Session;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Session;
using DndSolution.Application.Services.Rules;

namespace Core.Api.Mappings;

/// <summary>
/// Перевод игровой сессии между моделями и DTO
/// </summary>
public static class SessionMappings
{
    public static SessionDto ToDto(this SessionView view) => new()
    {
        CurrentHp = view.State.CurrentHp,
        TempHp = view.State.TempHp,
        HitDiceSpent = view.State.HitDiceSpent,
        DeathSuccesses = view.State.DeathSuccesses,
        DeathFailures = view.State.DeathFailures,
        IsStable = view.State.IsStable,
        IsDead = view.State.IsDead,
        Conditions = view.State.Conditions.Select(c => (int)c).ToList(),
        Exhaustion = view.State.Exhaustion,
        MaxHp = SessionRules.MaxHp(view.Context, view.State.Exhaustion),
        BaseMaxHp = view.Context.BaseMaxHp,
        HitDie = view.Context.HitDie,
        HitDiceTotal = view.Context.Level,
        ConModifier = view.Context.ConModifier,
    };

    public static SessionStateDto ToDto(this SessionState state) => new()
    {
        CurrentHp = state.CurrentHp,
        TempHp = state.TempHp,
        HitDiceSpent = state.HitDiceSpent,
        DeathSuccesses = state.DeathSuccesses,
        DeathFailures = state.DeathFailures,
        IsStable = state.IsStable,
        IsDead = state.IsDead,
        Conditions = state.Conditions.Select(c => (int)c).ToList(),
        Exhaustion = state.Exhaustion,
    };

    public static SessionEventDto ToDto(this SessionEvent sessionEvent)
    {
        var dto = new SessionEventDto
        {
            Id = sessionEvent.Id,
            CreatedAt = sessionEvent.CreatedAt,
            Type = TypeOf(sessionEvent.Action),
            Before = sessionEvent.Result.Before.ToDto(),
            After = sessionEvent.Result.State.ToDto(),
            Outcomes = sessionEvent.Result.Outcomes.Select(o => JsonNamingPolicy.CamelCase.ConvertName(o.ToString())).ToList(),
            Healed = sessionEvent.Result.Healed,
            Absorbed = sessionEvent.Result.Absorbed,
        };

        return sessionEvent.Action switch
        {
            SessionAction.Damage a => Copy(dto, amount: a.Amount, critical: a.Critical),
            SessionAction.Heal a => Copy(dto, amount: a.Amount),
            SessionAction.TempHp a => Copy(dto, amount: a.Amount),
            SessionAction.SetHp a => Copy(dto, amount: a.Amount),
            SessionAction.DeathSave a => Copy(dto, roll: a.Roll),
            SessionAction.SpendHitDie a => Copy(dto, roll: a.Roll),
            SessionAction.SetCondition a => Copy(dto, condition: (int)a.Condition, active: a.Active),
            SessionAction.SetExhaustion a => Copy(dto, level: a.Level),
            _ => dto,
        };
    }

    /// <summary>
    /// Действие из запроса. Не хватает нужного поля — ошибка правил с именем поля
    /// </summary>
    public static SessionAction ToAction(this SessionActionRequest request)
    {
        int Required(int? value, string field) =>
            value ?? throw new SessionRulesException(field, "Поле обязательно для этого действия");

        return request.Type switch
        {
            "damage" => new SessionAction.Damage(Required(request.Amount, "Amount"), request.Critical ?? false),
            "heal" => new SessionAction.Heal(Required(request.Amount, "Amount")),
            "tempHp" => new SessionAction.TempHp(Required(request.Amount, "Amount")),
            "setHp" => new SessionAction.SetHp(Required(request.Amount, "Amount")),
            "deathSave" => new SessionAction.DeathSave(Required(request.Roll, "Roll")),
            "stabilize" => new SessionAction.Stabilize(),
            "spendHitDie" => new SessionAction.SpendHitDie(Required(request.Roll, "Roll")),
            "longRest" => new SessionAction.LongRest(),
            "setCondition" => new SessionAction.SetCondition(
                (Condition)Required(request.Condition, "Condition"),
                request.Active ?? throw new SessionRulesException("Active", "Поле обязательно для этого действия")),
            "setExhaustion" => new SessionAction.SetExhaustion(Required(request.Level, "Level")),
            _ => throw new SessionRulesException("Type", "Неизвестное действие"),
        };
    }

    private static string TypeOf(SessionAction action) =>
        JsonNamingPolicy.CamelCase.ConvertName(action.GetType().Name);

    private static SessionEventDto Copy(
        SessionEventDto dto, int? amount = null, bool? critical = null, int? roll = null,
        int? condition = null, bool? active = null, int? level = null) => new()
    {
        Id = dto.Id,
        CreatedAt = dto.CreatedAt,
        Type = dto.Type,
        Before = dto.Before,
        After = dto.After,
        Outcomes = dto.Outcomes,
        Healed = dto.Healed,
        Absorbed = dto.Absorbed,
        Amount = amount,
        Critical = critical,
        Roll = roll,
        Condition = condition,
        Active = active,
        Level = level,
    };
}
