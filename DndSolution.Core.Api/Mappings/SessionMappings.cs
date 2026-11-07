using System.Text.Json;
using Core.Sdk.Dtos.Session;
using DndSolution.Application.Abstractions;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Rules;
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
        Inventory = view.State.Inventory.ToDto(),
        MaxHp = SessionRules.MaxHp(view.Context, view.State.Exhaustion),
        BaseMaxHp = view.Context.BaseMaxHp,
        HitDie = view.Context.HitDie,
        HitDiceTotal = view.Context.Level,
        ConModifier = view.Context.ConModifier,
        Equipment = EquipmentRules.Evaluate(view.Context, view.State.Inventory).ToDto(),
        Profile = view.Context.Equipment!.ToDto(),
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
        Inventory = state.Inventory.ToDto(),
    };

    public static InventoryDto ToDto(this InventoryState inventory) => new()
    {
        Currency = new CurrencyDto
        {
            Cp = inventory.Currency.Cp,
            Sp = inventory.Currency.Sp,
            Ep = inventory.Currency.Ep,
            Gp = inventory.Currency.Gp,
            Pp = inventory.Currency.Pp,
        },
        Items = inventory.Items.Select(i => new InventoryItemDto
        {
            Id = i.Id,
            Key = i.Key,
            Name = i.Name,
            Weight = i.Weight,
            Note = i.Note,
            Quantity = i.Quantity,
            Equipped = i.Equipped,
        }).ToList(),
    };

    public static EquipmentSummaryDto ToDto(this EquipmentSummary summary) => new()
    {
        ArmorClass = summary.ArmorClass,
        ArmorSource = summary.ArmorSource,
        Speed = summary.Speed,
        StealthDisadvantage = summary.StealthDisadvantage,
        Weight = summary.Weight,
        Capacity = summary.Capacity,
        Encumbered = summary.Encumbered,
        Warnings = summary.Warnings.ToList(),
        Attacks = summary.Attacks.Select(a => new WeaponAttackDto
        {
            ItemId = a.ItemId,
            Name = a.Name,
            AttackBonus = a.AttackBonus,
            Damage = a.Damage,
            DamageType = a.DamageType,
            Versatile = a.Versatile,
            Range = a.Range?.ToList(),
            Proficient = a.Proficient,
            AmmoItemId = a.AmmoItemId,
            AmmoCount = a.AmmoCount,
        }).ToList(),
    };

    public static EquipmentProfileDto ToDto(this EquipmentProfile profile)
    {
        static Dictionary<string, int> Abilities(AbilityScores s) => new()
        {
            ["strength"] = s.Strength,
            ["dexterity"] = s.Dexterity,
            ["constitution"] = s.Constitution,
            ["intelligence"] = s.Intelligence,
            ["wisdom"] = s.Wisdom,
            ["charisma"] = s.Charisma,
        };

        return new EquipmentProfileDto
        {
            Scores = Abilities(profile.Scores),
            Modifiers = Abilities(profile.Modifiers),
            ProficiencyBonus = profile.ProficiencyBonus,
            BaseSpeed = profile.BaseSpeed,
            Unarmored = (int)profile.Unarmored,
            ArmorProficiencies = profile.ArmorProficiencies.ToList(),
            WeaponProficiencies = profile.WeaponProficiencies.ToList(),
            IgnoresHeavyArmorSpeed = profile.IgnoresHeavyArmorSpeed,
        };
    }

    public static SessionEventDto ToDto(this SessionEvent sessionEvent)
    {
        var action = sessionEvent.Action;
        var currency = action as SessionAction.ChangeCurrency;

        return new SessionEventDto
        {
            Id = sessionEvent.Id,
            CreatedAt = sessionEvent.CreatedAt,
            Type = TypeOf(action),
            Before = sessionEvent.Result.Before.ToDto(),
            After = sessionEvent.Result.State.ToDto(),
            Outcomes = sessionEvent.Result.Outcomes.Select(o => JsonNamingPolicy.CamelCase.ConvertName(o.ToString())).ToList(),
            Healed = sessionEvent.Result.Healed,
            Absorbed = sessionEvent.Result.Absorbed,
            Amount = action switch
            {
                SessionAction.Damage a => a.Amount,
                SessionAction.Heal a => a.Amount,
                SessionAction.TempHp a => a.Amount,
                SessionAction.SetHp a => a.Amount,
                _ => null,
            },
            Critical = (action as SessionAction.Damage)?.Critical,
            Roll = action switch
            {
                SessionAction.DeathSave a => a.Roll,
                SessionAction.SpendHitDie a => a.Roll,
                SessionAction.DrinkPotion a => a.Roll,
                _ => null,
            },
            Condition = action is SessionAction.SetCondition c ? (int)c.Condition : null,
            Active = (action as SessionAction.SetCondition)?.Active,
            Level = (action as SessionAction.SetExhaustion)?.Level,
            ItemId = action switch
            {
                SessionAction.AddItem a => a.ItemId,
                SessionAction.RemoveItem a => a.ItemId,
                SessionAction.SetEquipped a => a.ItemId,
                SessionAction.DrinkPotion a => a.ItemId,
                SessionAction.UseAmmo a => a.ItemId,
                _ => null,
            },
            Key = (action as SessionAction.AddItem)?.Key,
            Name = (action as SessionAction.AddItem)?.Name,
            Weight = (action as SessionAction.AddItem)?.Weight,
            Note = (action as SessionAction.AddItem)?.Note,
            Quantity = action switch
            {
                SessionAction.AddItem a => a.Quantity,
                SessionAction.RemoveItem a => a.Quantity,
                SessionAction.UseAmmo a => a.Quantity,
                _ => null,
            },
            Equipped = (action as SessionAction.SetEquipped)?.Equipped,
            Cp = currency?.Cp,
            Sp = currency?.Sp,
            Ep = currency?.Ep,
            Gp = currency?.Gp,
            Pp = currency?.Pp,
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
            "addItem" => new SessionAction.AddItem(
                ItemId(request), request.Key, request.Quantity ?? 1, request.Name, request.Weight, request.Note),
            "removeItem" => new SessionAction.RemoveItem(ItemId(request), Required(request.Quantity, "Quantity")),
            "setEquipped" => new SessionAction.SetEquipped(
                ItemId(request),
                request.Equipped ?? throw new SessionRulesException("Equipped", "Поле обязательно для этого действия")),
            "changeCurrency" => new SessionAction.ChangeCurrency(
                request.Cp ?? 0, request.Sp ?? 0, request.Ep ?? 0, request.Gp ?? 0, request.Pp ?? 0),
            "drinkPotion" => new SessionAction.DrinkPotion(ItemId(request), Required(request.Roll, "Roll")),
            "useAmmo" => new SessionAction.UseAmmo(ItemId(request), request.Quantity ?? 1),
            _ => throw new SessionRulesException("Type", "Неизвестное действие"),
        };
    }

    private static Guid ItemId(SessionActionRequest request) =>
        request.ItemId is { } id && id != Guid.Empty
            ? id
            : throw new SessionRulesException("ItemId", "Поле обязательно для этого действия");

    private static string TypeOf(SessionAction action) =>
        JsonNamingPolicy.CamelCase.ConvertName(action.GetType().Name);
}
