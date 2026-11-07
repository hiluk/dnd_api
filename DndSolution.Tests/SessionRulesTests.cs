using System.Text.Json;
using System.Text.Json.Nodes;
using DndSolution.Application.Models.Enums;
using DndSolution.Application.Models.Models.Session;
using DndSolution.Application.Services.Rules;

namespace DndSolution.Tests;

/// <summary>
/// Правила сессии по общему набору примеров docs/session-rules-cases.json.
/// Тот же файл проверяет приложение, поэтому правила на сервере и в клиенте совпадают.
/// </summary>
public class SessionRulesTests
{
    private static readonly JsonArray Cases =
        JsonNode.Parse(File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "session-rules-cases.json")))!
            ["cases"]!.AsArray();

    public static IEnumerable<object[]> CaseNames() => Cases.Select(c => new object[] { c!["name"]!.GetValue<string>() });

    [Theory]
    [MemberData(nameof(CaseNames))]
    public void Пример(string name)
    {
        var data = Cases.Single(c => c!["name"]!.GetValue<string>() == name)!;
        var context = Context(data["context"]!);
        var state = State(data["state"]!);
        var action = Action(data["action"]!);

        if (data["error"] is { } error)
        {
            var exception = Assert.Throws<SessionRulesException>(() => SessionRules.Apply(context, state, action));
            Assert.Equal(error.GetValue<string>(), exception.Field);
            return;
        }

        var result = SessionRules.Apply(context, state, action);
        var expect = data["expect"]!;

        Assert.Equal(State(expect["state"]!), result.State, new StateComparer());
        Assert.Equal(
            expect["outcomes"]!.AsArray().Select(o => o!.GetValue<string>()).ToList(),
            result.Outcomes.Select(o => JsonNamingPolicy.CamelCase.ConvertName(o.ToString())).ToList());
        Assert.Equal(expect["healed"]!.GetValue<int>(), result.Healed);
        Assert.Equal(expect["absorbed"]!.GetValue<int>(), result.Absorbed);
    }

    [Fact]
    public void Примеры_покрывают_все_действия()
    {
        var types = Cases.Select(c => c!["action"]!["type"]!.GetValue<string>()).ToHashSet();
        var expected = typeof(SessionAction).GetNestedTypes()
            .Select(t => JsonNamingPolicy.CamelCase.ConvertName(t.Name))
            .ToHashSet();

        Assert.Equal(expected, types);
    }

    private static SessionContext Context(JsonNode node) => new(
        node["baseMaxHp"]!.GetValue<int>(),
        node["hitDie"]!.GetValue<int>(),
        node["level"]!.GetValue<int>(),
        node["conModifier"]!.GetValue<int>());

    private static SessionState State(JsonNode node) => new()
    {
        CurrentHp = node["currentHp"]!.GetValue<int>(),
        TempHp = node["tempHp"]!.GetValue<int>(),
        HitDiceSpent = node["hitDiceSpent"]!.GetValue<int>(),
        DeathSuccesses = node["deathSuccesses"]!.GetValue<int>(),
        DeathFailures = node["deathFailures"]!.GetValue<int>(),
        IsStable = node["isStable"]!.GetValue<bool>(),
        IsDead = node["isDead"]!.GetValue<bool>(),
        Conditions = node["conditions"]!.AsArray().Select(c => (Condition)c!.GetValue<int>()).ToList(),
        Exhaustion = node["exhaustion"]!.GetValue<int>(),
    };

    private static SessionAction Action(JsonNode node)
    {
        int Int(string key) => node[key]!.GetValue<int>();

        return node["type"]!.GetValue<string>() switch
        {
            "damage" => new SessionAction.Damage(Int("amount"), node["critical"]?.GetValue<bool>() ?? false),
            "heal" => new SessionAction.Heal(Int("amount")),
            "tempHp" => new SessionAction.TempHp(Int("amount")),
            "setHp" => new SessionAction.SetHp(Int("amount")),
            "deathSave" => new SessionAction.DeathSave(Int("roll")),
            "stabilize" => new SessionAction.Stabilize(),
            "spendHitDie" => new SessionAction.SpendHitDie(Int("roll")),
            "longRest" => new SessionAction.LongRest(),
            "setCondition" => new SessionAction.SetCondition((Condition)Int("condition"), node["active"]!.GetValue<bool>()),
            "setExhaustion" => new SessionAction.SetExhaustion(Int("level")),
            var type => throw new ArgumentException($"Неизвестное действие {type}"),
        };
    }

    /// <summary>
    /// Состояния равны с учётом списка состояний (record сравнивает списки по ссылке)
    /// </summary>
    private sealed class StateComparer : IEqualityComparer<SessionState>
    {
        public bool Equals(SessionState? x, SessionState? y) =>
            x is not null && y is not null
            && x with { Conditions = [] } == y with { Conditions = [] }
            && x.Conditions.SequenceEqual(y.Conditions);

        public int GetHashCode(SessionState obj) => obj.CurrentHp;
    }
}
