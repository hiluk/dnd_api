using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Models.Models.Session;

namespace DndSolution.Application.Services.Rules;

/// <summary>
/// Правила игровой сессии по «Книге игрока» 5e: урон, лечение, спасброски от смерти,
/// отдых, состояния и истощение. Чистые функции; те же правила есть в приложении,
/// а общий набор примеров (session-rules-cases.json) проверяет, что они совпадают.
/// </summary>
public static class SessionRules
{
    public const int MaxAmount = 9999;
    public const int MaxExhaustion = 6;

    /// <summary>
    /// Максимум хитов с учётом истощения: с 4-го уровня он вдвое меньше
    /// </summary>
    public static int MaxHp(SessionContext context, int exhaustion) =>
        exhaustion >= 4 ? Math.Max(1, context.BaseMaxHp / 2) : context.BaseMaxHp;

    /// <summary>
    /// Без сознания и умирает: 0 хитов, не стабилизирован, жив
    /// </summary>
    public static bool IsDying(SessionState state) => state is { CurrentHp: 0, IsStable: false, IsDead: false };

    public static SessionResult Apply(SessionContext context, SessionState state, SessionAction action)
    {
        // Погибшему можно вернуть хиты, поменять состояния и разобрать снаряжение
        if (state.IsDead && action is not (SessionAction.SetHp or SessionAction.SetCondition
                or SessionAction.SetExhaustion or SessionAction.AddItem or SessionAction.RemoveItem
                or SessionAction.SetEquipped or SessionAction.ChangeCurrency or SessionAction.UseAmmo))
        {
            throw new SessionRulesException("Action", "Персонаж погиб. Верните ему хиты вручную, если его воскресили");
        }

        return action switch
        {
            SessionAction.Damage damage => Damage(context, state, damage),
            SessionAction.Heal heal => Heal(context, state, heal),
            SessionAction.TempHp temp => TempHp(state, temp),
            SessionAction.SetHp set => SetHp(context, state, set),
            SessionAction.DeathSave save => DeathSave(state, save),
            SessionAction.Stabilize => Stabilize(state),
            SessionAction.SpendHitDie die => SpendHitDie(context, state, die),
            SessionAction.LongRest => LongRest(context, state),
            SessionAction.SetCondition condition => SetCondition(state, condition),
            SessionAction.SetExhaustion exhaustion => SetExhaustion(context, state, exhaustion),
            SessionAction.AddItem add => Inventory(state, InventoryRules.Add(context, state.Inventory, add)),
            SessionAction.RemoveItem remove => Inventory(state, InventoryRules.Remove(state.Inventory, remove)),
            SessionAction.SetEquipped equip => Inventory(state, InventoryRules.SetEquipped(context, state.Inventory, equip)),
            SessionAction.ChangeCurrency currency => Inventory(state, InventoryRules.ChangeCurrency(state.Inventory, currency)),
            SessionAction.UseAmmo ammo => Inventory(state, InventoryRules.UseAmmo(context, state.Inventory, ammo)),
            SessionAction.DrinkPotion potion => DrinkPotion(context, state, potion),
            _ => throw new SessionRulesException("Action", "Неизвестное действие"),
        };
    }

    private static SessionResult Damage(SessionContext context, SessionState state, SessionAction.Damage action)
    {
        CheckAmount(action.Amount);
        var max = MaxHp(context, state.Exhaustion);
        var absorbed = Math.Min(state.TempHp, action.Amount);
        var rest = action.Amount - absorbed;
        var next = state with { TempHp = state.TempHp - absorbed };
        var outcomes = new List<SessionOutcome>();

        if (state.CurrentHp == 0)
        {
            // Урон по лежащему: провал спасброска, критический удар — два
            if (rest >= max)
            {
                next = Dead(next);
                outcomes.AddRange([SessionOutcome.InstantDeath, SessionOutcome.Died]);
            }
            else if (rest > 0)
            {
                next = next with
                {
                    IsStable = false,
                    DeathFailures = Math.Min(3, state.DeathFailures + (action.Critical ? 2 : 1)),
                };
                outcomes.Add(SessionOutcome.DeathSaveFailure);
                if (next.DeathFailures >= 3)
                {
                    next = Dead(next);
                    outcomes.Add(SessionOutcome.Died);
                }
            }
        }
        else if (rest >= state.CurrentHp)
        {
            // Остаток урона после 0 не меньше максимума — мгновенная смерть
            var overflow = rest - state.CurrentHp;
            next = next with { CurrentHp = 0, IsStable = false, DeathSuccesses = 0, DeathFailures = 0 };
            if (overflow >= max)
            {
                next = Dead(next);
                outcomes.AddRange([SessionOutcome.InstantDeath, SessionOutcome.Died]);
            }
            else
            {
                outcomes.Add(SessionOutcome.KnockedOut);
            }
        }
        else
        {
            next = next with { CurrentHp = state.CurrentHp - rest };
        }

        return new SessionResult(next, state, outcomes, Absorbed: absorbed);
    }

    private static SessionResult Heal(SessionContext context, SessionState state, SessionAction.Heal action)
    {
        CheckAmount(action.Amount);
        var max = MaxHp(context, state.Exhaustion);
        var hp = Math.Min(max, state.CurrentHp + action.Amount);
        var next = state with { CurrentHp = hp };
        var outcomes = new List<SessionOutcome>();

        if (state.CurrentHp == 0 && hp > 0)
        {
            next = ClearDeathSaves(next);
            outcomes.Add(SessionOutcome.Revived);
        }

        return new SessionResult(next, state, outcomes, Healed: hp - state.CurrentHp);
    }

    /// <summary>
    /// Временные хиты не складываются: остаётся большее значение
    /// </summary>
    private static SessionResult TempHp(SessionState state, SessionAction.TempHp action)
    {
        CheckAmount(action.Amount);
        return new SessionResult(state with { TempHp = Math.Max(state.TempHp, action.Amount) }, state, []);
    }

    private static SessionResult SetHp(SessionContext context, SessionState state, SessionAction.SetHp action)
    {
        var max = MaxHp(context, state.Exhaustion);
        if (action.Amount < 0 || action.Amount > max)
        {
            throw new SessionRulesException("Amount", $"Хиты должны быть от 0 до {max}");
        }

        var next = state with { CurrentHp = action.Amount };
        var outcomes = new List<SessionOutcome>();

        if (state.IsDead)
        {
            if (action.Amount == 0)
            {
                throw new SessionRulesException("Amount", "Чтобы воскресить персонажа, верните ему хотя бы 1 хит");
            }

            next = ClearDeathSaves(next) with
            {
                IsDead = false,
                Exhaustion = Math.Min(state.Exhaustion, MaxExhaustion - 1),
            };
            outcomes.Add(SessionOutcome.Resurrected);
        }
        else if (state.CurrentHp == 0 && action.Amount > 0)
        {
            next = ClearDeathSaves(next);
            outcomes.Add(SessionOutcome.Revived);
        }
        else if (state.CurrentHp > 0 && action.Amount == 0)
        {
            next = ClearDeathSaves(next);
            outcomes.Add(SessionOutcome.KnockedOut);
        }

        return new SessionResult(next, state, outcomes);
    }

    private static SessionResult DeathSave(SessionState state, SessionAction.DeathSave action)
    {
        if (!IsDying(state))
        {
            throw new SessionRulesException("Action", "Спасброски от смерти делают при 0 хитов, пока персонаж не стабилен");
        }

        CheckRoll(action.Roll, 20);
        var outcomes = new List<SessionOutcome>();
        SessionState next;

        if (action.Roll == 20)
        {
            // Встаёт с 1 хитом
            next = ClearDeathSaves(state) with { CurrentHp = 1 };
            outcomes.AddRange([SessionOutcome.NaturalTwenty, SessionOutcome.Revived]);
        }
        else if (action.Roll >= 10)
        {
            next = state with { DeathSuccesses = state.DeathSuccesses + 1 };
            outcomes.Add(SessionOutcome.DeathSaveSuccess);
            if (next.DeathSuccesses >= 3)
            {
                next = ClearDeathSaves(next) with { IsStable = true };
                outcomes.Add(SessionOutcome.Stabilized);
            }
        }
        else
        {
            var failures = action.Roll == 1 ? 2 : 1;
            next = state with { DeathFailures = Math.Min(3, state.DeathFailures + failures) };
            outcomes.Add(SessionOutcome.DeathSaveFailure);
            if (action.Roll == 1)
            {
                outcomes.Add(SessionOutcome.NaturalOne);
            }

            if (next.DeathFailures >= 3)
            {
                next = Dead(next);
                outcomes.Add(SessionOutcome.Died);
            }
        }

        return new SessionResult(next, state, outcomes);
    }

    private static SessionResult Stabilize(SessionState state)
    {
        if (!IsDying(state))
        {
            throw new SessionRulesException("Action", "Стабилизировать можно только умирающего персонажа");
        }

        return new SessionResult(ClearDeathSaves(state) with { IsStable = true }, state, [SessionOutcome.Stabilized]);
    }

    private static SessionResult SpendHitDie(SessionContext context, SessionState state, SessionAction.SpendHitDie action)
    {
        if (state.CurrentHp == 0)
        {
            throw new SessionRulesException("Action", "Кости хитов нельзя тратить без сознания");
        }

        if (state.HitDiceSpent >= context.Level)
        {
            throw new SessionRulesException("Action", "Свободных костей хитов не осталось");
        }

        CheckRoll(action.Roll, context.HitDie);
        var max = MaxHp(context, state.Exhaustion);
        var hp = Math.Min(max, state.CurrentHp + Math.Max(0, action.Roll + context.ConModifier));

        return new SessionResult(
            state with { CurrentHp = hp, HitDiceSpent = state.HitDiceSpent + 1 },
            state,
            [],
            Healed: hp - state.CurrentHp);
    }

    private static SessionResult LongRest(SessionContext context, SessionState state)
    {
        if (state.CurrentHp == 0)
        {
            throw new SessionRulesException("Action", "Для длинного отдыха нужен хотя бы 1 хит");
        }

        var exhaustion = Math.Max(0, state.Exhaustion - 1);
        var max = MaxHp(context, exhaustion);
        var regained = Math.Max(1, context.Level / 2);
        var next = ClearDeathSaves(state) with
        {
            CurrentHp = max,
            TempHp = 0,
            HitDiceSpent = Math.Max(0, state.HitDiceSpent - regained),
            Exhaustion = exhaustion,
        };

        return new SessionResult(next, state, [], Healed: Math.Max(0, max - state.CurrentHp));
    }

    private static SessionResult SetCondition(SessionState state, SessionAction.SetCondition action)
    {
        if (!Enum.IsDefined(action.Condition))
        {
            throw new SessionRulesException("Condition", "Неизвестное состояние");
        }

        var conditions = state.Conditions.Where(c => c != action.Condition).ToList();
        if (action.Active)
        {
            conditions.Add(action.Condition);
        }

        conditions.Sort();
        return new SessionResult(state with { Conditions = conditions }, state, []);
    }

    private static SessionResult SetExhaustion(SessionContext context, SessionState state, SessionAction.SetExhaustion action)
    {
        if (action.Level is < 0 or > MaxExhaustion)
        {
            throw new SessionRulesException("Level", $"Истощение должно быть от 0 до {MaxExhaustion}");
        }

        var max = MaxHp(context, action.Level);
        var next = state with { Exhaustion = action.Level, CurrentHp = Math.Min(state.CurrentHp, max) };
        var outcomes = new List<SessionOutcome>();

        if (action.Level == MaxExhaustion && !state.IsDead)
        {
            next = Dead(next);
            outcomes.Add(SessionOutcome.Died);
        }

        return new SessionResult(next, state, outcomes);
    }

    private static SessionResult Inventory(SessionState state, InventoryState inventory) =>
        new(state with { Inventory = inventory }, state, []);

    /// <summary>
    /// Зелье: лечение на выпавшее значение, как обычное лечение, и зелий на одно меньше
    /// </summary>
    private static SessionResult DrinkPotion(SessionContext context, SessionState state, SessionAction.DrinkPotion action)
    {
        var (item, definition) = InventoryRules.Find(context, state.Inventory, action.ItemId);
        if (definition?.Potion is null)
        {
            throw new SessionRulesException("ItemId", "Это не зелье");
        }

        var dice = DiceFormula.Parse(definition.Potion.Heal);
        if (action.Roll < dice.Min || action.Roll > dice.Max)
        {
            throw new SessionRulesException("Roll", $"На {dice} выпадает от {dice.Min} до {dice.Max}");
        }

        var healed = Heal(context, state, new SessionAction.Heal(action.Roll));
        var inventory = InventoryRules.Remove(state.Inventory, new SessionAction.RemoveItem(item.Id, 1));

        return healed with { State = healed.State with { Inventory = inventory } };
    }

    private static SessionState Dead(SessionState state) =>
        state with { IsDead = true, IsStable = false, CurrentHp = 0, DeathSuccesses = 0, DeathFailures = 0 };

    private static SessionState ClearDeathSaves(SessionState state) =>
        state with { DeathSuccesses = 0, DeathFailures = 0, IsStable = false };

    private static void CheckAmount(int amount)
    {
        if (amount is < 0 or > MaxAmount)
        {
            throw new SessionRulesException("Amount", $"Значение должно быть от 0 до {MaxAmount}");
        }
    }

    private static void CheckRoll(int roll, int faces)
    {
        if (roll < 1 || roll > faces)
        {
            throw new SessionRulesException("Roll", $"На к{faces} выпадает от 1 до {faces}");
        }
    }
}
