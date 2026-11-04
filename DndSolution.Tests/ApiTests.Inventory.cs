using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace DndSolution.Tests;

/// <summary>
/// Справочник предметов и снаряжение в сессии
/// </summary>
public partial class ApiTests
{
    [Fact]
    public async Task Справочник_предметов_доступен_без_входа()
    {
        var client = _factory.CreateClient();

        var items = (await client.GetFromJsonAsync<JsonArray>("/items"))!;

        Assert.True(items.Count > 100);
        var longsword = items.Single(i => i!["key"]!.GetValue<string>() == "longsword")!;
        Assert.Equal("weapon", longsword["category"]!.GetValue<string>());
        Assert.Equal("1d10", longsword["weapon"]!["versatile"]!.GetValue<string>());
        var pack = items.Single(i => i!["key"]!.GetValue<string>() == "explorers-pack")!;
        Assert.Equal("pack", pack["category"]!.GetValue<string>());
        Assert.NotEmpty(pack["pack"]!.AsArray());
    }

    [Fact]
    public async Task Доспех_щит_и_оружие_меняют_КД_и_атаки()
    {
        var (client, id) = await CharacterAsync();
        Guid mail = Guid.NewGuid(), shield = Guid.NewGuid(), sword = Guid.NewGuid();

        var before = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}/session"))!;
        var profile = before["profile"]!;
        Assert.True(profile["ignoresHeavyArmorSpeed"]!.GetValue<bool>());
        Assert.Contains("heavy", profile["armorProficiencies"]!.AsArray().Select(a => a!.GetValue<string>()));
        Assert.Equal("Без доспеха", before["equipment"]!["armorSource"]!.GetValue<string>());

        await ActAsync(client, id, Act("addItem", new { itemId = mail, key = "chain-mail", quantity = 1 }));
        await ActAsync(client, id, Act("addItem", new { itemId = shield, key = "shield", quantity = 1 }));
        await ActAsync(client, id, Act("addItem", new { itemId = sword, key = "longsword", quantity = 1 }));
        await ActAsync(client, id, Act("setEquipped", new { itemId = mail, equipped = true }));
        await ActAsync(client, id, Act("setEquipped", new { itemId = shield, equipped = true }));
        var response = await ActAsync(client, id, Act("setEquipped", new { itemId = sword, equipped = true }));

        var session = response["session"]!;
        var equipment = session["equipment"]!;
        Assert.Equal(18, equipment["armorClass"]!.GetValue<int>());
        Assert.Equal("Кольчуга, щит", equipment["armorSource"]!.GetValue<string>());
        Assert.True(equipment["stealthDisadvantage"]!.GetValue<bool>());
        Assert.Equal(64, equipment["weight"]!.GetValue<double>());
        Assert.Equal(profile["baseSpeed"]!.GetValue<int>(), equipment["speed"]!.GetValue<int>());

        var attack = equipment["attacks"]!.AsArray().Single()!;
        Assert.Equal(sword.ToString(), attack["itemId"]!.GetValue<string>());
        Assert.Equal("Длинный меч", attack["name"]!.GetValue<string>());
        Assert.True(attack["proficient"]!.GetValue<bool>());
        var strength = profile["modifiers"]!["strength"]!.GetValue<int>();
        Assert.Equal(strength + profile["proficiencyBonus"]!.GetValue<int>(), attack["attackBonus"]!.GetValue<int>());

        var items = session["inventory"]!["items"]!.AsArray();
        Assert.Equal(3, items.Count);
        Assert.All(items, i => Assert.True(i!["equipped"]!.GetValue<bool>()));

        // Журнал хранит, что именно надели
        Assert.Equal("setEquipped", response["event"]!["type"]!.GetValue<string>());
        Assert.Equal(sword.ToString(), response["event"]!["itemId"]!.GetValue<string>());
        Assert.True(response["event"]!["equipped"]!.GetValue<bool>());
    }

    [Fact]
    public async Task Зелье_лечит_и_тратится_монеты_и_стрелы()
    {
        var (client, id) = await CharacterAsync();
        Guid potion = Guid.NewGuid(), arrows = Guid.NewGuid();

        await ActAsync(client, id, Act("addItem", new { itemId = potion, key = "potion-healing", quantity = 2 }));
        await ActAsync(client, id, Act("addItem", new { itemId = arrows, key = "arrows", quantity = 20 }));
        await ActAsync(client, id, Act("damage", new { amount = 8 }));

        var drink = await ActAsync(client, id, Act("drinkPotion", new { itemId = potion, roll = 6 }));
        Assert.Equal(6, drink["event"]!["healed"]!.GetValue<int>());
        Assert.Equal(6, drink["event"]!["roll"]!.GetValue<int>());
        Assert.Equal(drink["session"]!["maxHp"]!.GetValue<int>() - 2, drink["session"]!["currentHp"]!.GetValue<int>());
        Assert.Equal(1, Item(drink, potion)["quantity"]!.GetValue<int>());

        var shot = await ActAsync(client, id, Act("useAmmo", new { itemId = arrows }));
        Assert.Equal(19, Item(shot, arrows)["quantity"]!.GetValue<int>());

        await ActAsync(client, id, Act("changeCurrency", new { gp = 10, sp = 5 }));
        var spent = await ActAsync(client, id, Act("changeCurrency", new { gp = -3 }));
        var currency = spent["session"]!["inventory"]!["currency"]!;
        Assert.Equal(7, currency["gp"]!.GetValue<int>());
        Assert.Equal(5, currency["sp"]!.GetValue<int>());
        Assert.Equal(-3, spent["event"]!["gp"]!.GetValue<int>());
        Assert.Equal(10, spent["event"]!["before"]!["inventory"]!["currency"]!["gp"]!.GetValue<int>());

        // Своё снаряжение переживает перезагрузку
        var session = (await client.GetFromJsonAsync<JsonObject>($"/characters/{id}/session"))!;
        Assert.Equal(2, session["inventory"]!["items"]!.AsArray().Count);
    }

    [Fact]
    public async Task Недоступные_действия_со_снаряжением()
    {
        var (client, id) = await CharacterAsync();
        var torch = Guid.NewGuid();
        await ActAsync(client, id, Act("addItem", new { itemId = torch, key = "torch", quantity = 1 }));

        async Task<string> Rejected(object action)
        {
            var response = await client.PostAsJsonAsync($"/characters/{id}/actions", action);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var problem = (await response.Content.ReadFromJsonAsync<JsonObject>())!;
            return string.Join(",", problem["errors"]!.AsObject().Select(e => e.Key));
        }

        Assert.Equal("Key", await Rejected(Act("addItem", new { itemId = Guid.NewGuid(), key = "explorers-pack" })));
        Assert.Equal("ItemId", await Rejected(Act("setEquipped", new { itemId = torch, equipped = true })));
        Assert.Equal("ItemId", await Rejected(Act("drinkPotion", new { itemId = torch, roll = 5 })));
        Assert.Equal("Roll", await Rejected(Act("drinkPotion", new { itemId = torch })));
        Assert.Equal("ItemId", await Rejected(Act("removeItem", new { quantity = 1 })));
        Assert.Equal("Currency", await Rejected(Act("changeCurrency", new { gp = -1 })));
    }

    private static JsonNode Item(JsonObject response, Guid id) =>
        response["session"]!["inventory"]!["items"]!.AsArray()
            .Single(i => i!["id"]!.GetValue<string>() == id.ToString())!;
}
