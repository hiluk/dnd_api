using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace DndSolution.Tests;

/// <summary>
/// Игровая сессия: состояние, действия, повтор действий и журнал
/// </summary>
public partial class ApiTests
{
    private static object Act(string type, object? fields = null, Guid? id = null)
    {
        var node = JsonNode.Parse(System.Text.Json.JsonSerializer.Serialize(fields ?? new { }))!.AsObject();
        node["id"] = (id ?? Guid.NewGuid()).ToString();
        node["createdAt"] = DateTime.UtcNow.ToString("O");
        node["type"] = type;
        return node;
    }

    private static async Task<JsonObject> ActAsync(HttpClient client, string id, object action)
    {
        var response = await client.PostAsJsonAsync($"/characters/{id}/actions", action);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<JsonObject>())!;
    }

    [Fact]
    public async Task Новая_сессия_начинается_с_полных_хитов()
    {
        var (client, id) = await CharacterAsync();
        var sheet = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}"))!["sheet"]!;

        var session = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}/session"))!;

        // Дварф-воин: к10, Телосложение 13 + 2 от расы = 15 (+2)
        Assert.Equal(sheet["hitPoints"]!.GetValue<int>(), session["currentHp"]!.GetValue<int>());
        Assert.Equal(session["currentHp"]!.GetValue<int>(), session["maxHp"]!.GetValue<int>());
        Assert.Equal(10, session["hitDie"]!.GetValue<int>());
        Assert.Equal(1, session["hitDiceTotal"]!.GetValue<int>());
        Assert.Equal(2, session["conModifier"]!.GetValue<int>());
        Assert.Empty((await client.GetFromJsonAsync<JsonArray>($"/characters/{id}/events"))!);
    }

    [Fact]
    public async Task Урон_до_нуля_спасброски_и_стабилизация()
    {
        var (client, id) = await CharacterAsync();

        await ActAsync(client, id, Act("tempHp", new { amount = 3 }));
        var hit = await ActAsync(client, id, Act("damage", new { amount = 5 }));
        Assert.Equal(3, hit["event"]!["absorbed"]!.GetValue<int>());
        Assert.Equal(10, hit["session"]!["currentHp"]!.GetValue<int>());

        var down = await ActAsync(client, id, Act("damage", new { amount = 10 }));
        Assert.Equal(0, down["session"]!["currentHp"]!.GetValue<int>());
        Assert.Contains("knockedOut", down["event"]!["outcomes"]!.AsArray().Select(o => o!.GetValue<string>()));

        await ActAsync(client, id, Act("deathSave", new { roll = 12 }));
        await ActAsync(client, id, Act("deathSave", new { roll = 15 }));
        var stable = await ActAsync(client, id, Act("deathSave", new { roll = 18 }));

        Assert.True(stable["session"]!["isStable"]!.GetValue<bool>());

        // Журнал читается со всеми видами действий (в том числе спасбросками)
        var events = (await client.GetFromJsonAsync<JsonArray>($"/characters/{id}/events"))!;
        Assert.Equal(6, events.Count);
        Assert.Equal(18, events[0]!["roll"]!.GetValue<int>());
        Assert.Equal("deathSave", events[0]!["type"]!.GetValue<string>());
        Assert.Equal(
            new[] { "deathSaveSuccess", "stabilized" },
            stable["event"]!["outcomes"]!.AsArray().Select(o => o!.GetValue<string>()));
    }

    [Fact]
    public async Task Повтор_действия_не_применяется_дважды()
    {
        var (client, id) = await CharacterAsync();
        var actionId = Guid.NewGuid();

        var first = await ActAsync(client, id, Act("damage", new { amount = 4 }, actionId));
        var repeat = await ActAsync(client, id, Act("damage", new { amount = 4 }, actionId));

        Assert.Equal(
            first["session"]!["currentHp"]!.GetValue<int>(),
            repeat["session"]!["currentHp"]!.GetValue<int>());
        Assert.Equal(actionId.ToString(), repeat["event"]!["id"]!.GetValue<string>());
        Assert.Single((await client.GetFromJsonAsync<JsonArray>($"/characters/{id}/events"))!);
    }

    [Fact]
    public async Task Недоступное_действие_возвращает_понятную_ошибку()
    {
        var (client, id) = await CharacterAsync();

        var save = await client.PostAsJsonAsync($"/characters/{id}/actions", Act("deathSave", new { roll = 12 }));
        Assert.Equal(HttpStatusCode.BadRequest, save.StatusCode);
        var problem = (await save.Content.ReadFromJsonAsync<JsonObject>())!;
        Assert.Equal("Действие недоступно", problem["title"]!.GetValue<string>());
        Assert.NotNull(problem["errors"]!["Action"]);

        var missing = await client.PostAsJsonAsync($"/characters/{id}/actions", Act("damage"));
        Assert.Equal(HttpStatusCode.BadRequest, missing.StatusCode);
        Assert.NotNull((await missing.Content.ReadFromJsonAsync<JsonObject>())!["errors"]!["Amount"]);

        var unknown = await client.PostAsJsonAsync($"/characters/{id}/actions", Act("fireball"));
        Assert.Equal(HttpStatusCode.BadRequest, unknown.StatusCode);

        // Отклонённые действия в журнал не попадают
        Assert.Empty((await client.GetFromJsonAsync<JsonArray>($"/characters/{id}/events"))!);
    }

    [Fact]
    public async Task Отдых_состояния_и_истощение()
    {
        var (client, id) = await CharacterAsync();
        await ActAsync(client, id, Act("damage", new { amount = 8 }));

        var die = await ActAsync(client, id, Act("spendHitDie", new { roll = 3 }));
        Assert.Equal(5, die["event"]!["healed"]!.GetValue<int>());
        Assert.Equal(1, die["session"]!["hitDiceSpent"]!.GetValue<int>());

        var noDice = await client.PostAsJsonAsync($"/characters/{id}/actions", Act("spendHitDie", new { roll = 3 }));
        Assert.Equal(HttpStatusCode.BadRequest, noDice.StatusCode);

        await ActAsync(client, id, Act("setCondition", new { condition = 10, active = true }));
        var exhausted = await ActAsync(client, id, Act("setExhaustion", new { level = 4 }));
        var session = exhausted["session"]!;
        Assert.Equal(new[] { 10 }, session["conditions"]!.AsArray().Select(c => c!.GetValue<int>()));
        Assert.Equal(session["baseMaxHp"]!.GetValue<int>() / 2, session["maxHp"]!.GetValue<int>());

        var rested = (await ActAsync(client, id, Act("longRest")))["session"]!;
        Assert.Equal(3, rested["exhaustion"]!.GetValue<int>());
        Assert.Equal(rested["baseMaxHp"]!.GetValue<int>(), rested["currentHp"]!.GetValue<int>());
        Assert.Equal(0, rested["hitDiceSpent"]!.GetValue<int>());
    }

    [Fact]
    public async Task Журнал_от_новых_к_старым_с_подгрузкой()
    {
        var (client, id) = await CharacterAsync();
        foreach (var amount in new[] { 1, 2, 3 })
        {
            await ActAsync(client, id, Act("damage", new { amount }));
        }

        var events = (await client.GetFromJsonAsync<JsonArray>($"/characters/{id}/events?limit=2"))!;
        Assert.Equal(new[] { 3, 2 }, events.Select(e => e!["amount"]!.GetValue<int>()));
        Assert.Equal("damage", events[0]!["type"]!.GetValue<string>());
        Assert.Equal(
            events[0]!["before"]!["currentHp"]!.GetValue<int>() - 3,
            events[0]!["after"]!["currentHp"]!.GetValue<int>());
    }

    [Fact]
    public async Task Чужую_сессию_не_видно_и_не_изменить()
    {
        var (owner, id) = await CharacterAsync();
        var (stranger, _) = await LoggedClientAsync();
        await ActAsync(owner, id, Act("damage", new { amount = 2 }));

        Assert.Equal(HttpStatusCode.NotFound, (await stranger.GetAsync($"/characters/{id}/session")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await stranger.GetAsync($"/characters/{id}/events")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await stranger.PostAsJsonAsync($"/characters/{id}/actions", Act("heal", new { amount = 5 }))).StatusCode);
    }

    [Fact]
    public async Task Удаление_персонажа_удаляет_сессию()
    {
        var (client, id) = await CharacterAsync();
        await ActAsync(client, id, Act("damage", new { amount = 2 }));

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/characters/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/characters/{id}/session")).StatusCode);
    }
}
