using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Tests;

public class ApiTests : IClassFixture<ApiFactory>
{
    private const string Password = "Passw0rd";
    private readonly ApiFactory _factory;

    public ApiTests(ApiFactory factory)
    {
        _factory = factory;
    }

    private static object Stats(int str, int dex, int con, int @int, int wis, int cha) =>
        new { strength = str, dexterity = dex, constitution = con, intelligence = @int, wisdom = wis, charisma = cha };

    /// <summary>
    /// Дварф-воин, стандартный набор
    /// </summary>
    private static object NewCharacter(string name) => new
    {
        name,
        characterRace = 3,
        characterClass = 1,
        generationMethod = 1,
        baseStats = Stats(15, 14, 13, 12, 10, 8),
        bonusChoices = Array.Empty<int>(),
    };

    private async Task<(HttpClient Client, string RefreshToken)> LoggedClientAsync()
    {
        var client = _factory.CreateClient();
        var login = $"user{Guid.NewGuid():N}"[..16];

        var register = await client.PostAsJsonAsync("/register", new { email = $"{login}@test.ru", login, password = Password });
        Assert.Equal(HttpStatusCode.OK, register.StatusCode);

        var response = await client.PostAsJsonAsync("/login", new { login, password = Password });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var tokens = (await response.Content.ReadFromJsonAsync<JsonObject>())!;

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens["accessToken"]!.GetValue<string>());
        return (client, tokens["refreshToken"]!.GetValue<string>());
    }

    [Fact]
    public async Task Справочники_заполнены_и_доступны_без_авторизации()
    {
        var client = _factory.CreateClient();

        var races = await client.GetFromJsonAsync<JsonArray>("/races");
        var classes = await client.GetFromJsonAsync<JsonArray>("/classes");

        Assert.Equal(
            ["Человек", "Полуэльф", "Дварф", "Эльф", "Полурослик", "Драконорождённый", "Гном", "Полуорк", "Тифлинг"],
            races!.Select(x => x!["name"]!.GetValue<string>()));
        Assert.Equal(
            ["Воин", "Варвар", "Следопыт", "Плут", "Бард", "Жрец", "Друид", "Монах", "Паладин", "Чародей", "Колдун", "Волшебник"],
            classes!.Select(x => x!["name"]!.GetValue<string>()));
    }

    [Fact]
    public async Task Справочники_нельзя_менять_обычному_пользователю()
    {
        var (client, _) = await LoggedClientAsync();

        var response = await client.PostAsJsonAsync("/races/save-new", new { });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Неверный_пароль_возвращает_401_с_описанием()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/login", new { login = "nobody", password = "x" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.Equal("Неверный логин или пароль", problem!["detail"]!.GetValue<string>());
    }

    [Fact]
    public async Task Повторная_регистрация_возвращает_ошибки_валидации()
    {
        var client = _factory.CreateClient();
        var body = new { email = "dup@test.ru", login = "duplicate", password = Password };

        await client.PostAsJsonAsync("/register", body);
        var response = await client.PostAsJsonAsync("/register", body);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<JsonObject>();
        Assert.NotNull(problem!["errors"]!["DuplicateUserName"]);
    }

    [Fact]
    public async Task Refresh_токен_выдаёт_новую_пару()
    {
        var (client, refreshToken) = await LoggedClientAsync();

        var ok = await client.PostAsJsonAsync("/refresh", new { refreshToken });
        var bad = await client.PostAsJsonAsync("/refresh", new { refreshToken = "garbage" });

        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, bad.StatusCode);
    }

    [Fact]
    public async Task Персонажи_без_токена_недоступны()
    {
        var response = await _factory.CreateClient().GetAsync("/characters");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Создание_получение_и_удаление_персонажа()
    {
        var (client, _) = await LoggedClientAsync();

        var created = await client.PostAsJsonAsync("/characters/create", NewCharacter("Леголас"));
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var id = (await created.Content.ReadFromJsonAsync<JsonObject>())!["id"]!.GetValue<string>();

        var list = await client.GetFromJsonAsync<JsonArray>("/characters");
        Assert.Single(list!);

        var character = await client.GetFromJsonAsync<JsonObject>($"/characters/{id}");
        var stats = character!["baseStats"]!;
        Assert.Equal("Леголас", character["name"]!.GetValue<string>());
        Assert.Equal(14, stats["dexterity"]!.GetValue<int>());
        Assert.Equal(13, stats["constitution"]!.GetValue<int>());

        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/characters/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/characters/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.DeleteAsync($"/characters/{id}")).StatusCode);
    }

    [Fact]
    public async Task Чужого_персонажа_не_видно()
    {
        var (owner, _) = await LoggedClientAsync();
        var (stranger, _) = await LoggedClientAsync();

        var created = await owner.PostAsJsonAsync("/characters/create", NewCharacter("Гимли"));
        var id = (await created.Content.ReadFromJsonAsync<JsonObject>())!["id"]!.GetValue<string>();

        Assert.Equal(HttpStatusCode.NotFound, (await stranger.GetAsync($"/characters/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await stranger.DeleteAsync($"/characters/{id}")).StatusCode);
        Assert.Empty((await stranger.GetFromJsonAsync<JsonArray>("/characters"))!);
    }

    [Fact]
    public async Task Некорректный_персонаж_отклоняется()
    {
        var (client, _) = await LoggedClientAsync();

        var response = await client.PostAsJsonAsync("/characters/create", new
        {
            name = "",
            characterRace = 1,
            characterClass = 42,
            generationMethod = 1,
            baseStats = Stats(15, 14, 13, 12, 10, 8),
            bonusChoices = Array.Empty<int>(),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Лист_персонажа_считается_сервером()
    {
        var (client, _) = await LoggedClientAsync();

        var created = await client.PostAsJsonAsync("/characters/create", NewCharacter("Торин"));
        var character = (await created.Content.ReadFromJsonAsync<JsonObject>())!;
        var sheet = character["sheet"]!;

        Assert.Equal(1, character["level"]!.GetValue<int>());
        Assert.Equal(13, character["baseStats"]!["constitution"]!.GetValue<int>());
        Assert.Equal(15, sheet["stats"]!["constitution"]!.GetValue<int>());
        Assert.Equal(2, sheet["modifiers"]!["constitution"]!.GetValue<int>());
        Assert.Equal(12, sheet["hitPoints"]!.GetValue<int>());
        Assert.Equal(12, sheet["armorClass"]!.GetValue<int>());
        Assert.Equal(25, sheet["speed"]!.GetValue<int>());
        Assert.Equal(6, sheet["savingThrows"]!.AsArray().Count);
    }

    [Fact]
    public async Task Превью_не_сохраняет_персонажа()
    {
        var (client, _) = await LoggedClientAsync();

        var response = await client.PostAsJsonAsync("/characters/preview", NewCharacter("Черновик"));
        var sheet = (await response.Content.ReadFromJsonAsync<JsonObject>())!;

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(12, sheet["hitPoints"]!.GetValue<int>());
        Assert.Empty((await client.GetFromJsonAsync<JsonArray>("/characters"))!);
    }

    [Fact]
    public async Task Нарушение_правил_возвращает_понятную_ошибку()
    {
        var (client, _) = await LoggedClientAsync();

        var response = await client.PostAsJsonAsync("/characters/preview", new
        {
            characterRace = 3,
            characterClass = 1,
            generationMethod = 2,
            baseStats = Stats(15, 15, 15, 9, 8, 8),
            bonusChoices = Array.Empty<int>(),
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = (await response.Content.ReadFromJsonAsync<JsonObject>())!;
        Assert.Equal("Потрачено 28 очков из 27", problem["errors"]!["BaseStats"]![0]!.GetValue<string>());
    }

    [Fact]
    public async Task Полуэльф_с_выбором_бонусов_и_покупкой_очков()
    {
        var (client, _) = await LoggedClientAsync();

        var response = await client.PostAsJsonAsync("/characters/create", new
        {
            name = "Танис",
            characterRace = 2,
            characterClass = 3,
            generationMethod = 2,
            baseStats = Stats(13, 15, 14, 10, 12, 8),
            bonusChoices = new[] { 2, 5 },
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var stats = (await response.Content.ReadFromJsonAsync<JsonObject>())!["sheet"]!["stats"]!;
        Assert.Equal(16, stats["dexterity"]!.GetValue<int>());
        Assert.Equal(13, stats["wisdom"]!.GetValue<int>());
        Assert.Equal(10, stats["charisma"]!.GetValue<int>());
    }

    [Fact]
    public async Task Создание_по_броскам_сервера()
    {
        var (client, _) = await LoggedClientAsync();

        var roll = (await (await client.PostAsync("/abilities/roll", null)).Content.ReadFromJsonAsync<JsonObject>())!;
        var values = roll["values"]!.AsArray().Select(v => v!.GetValue<int>()).ToArray();
        Assert.Equal(6, values.Length);
        Assert.All(values, v => Assert.InRange(v, 3, 18));

        object Draft(int[] scores, string? token) => new
        {
            name = "Везунчик",
            characterRace = 1,
            characterClass = 12,
            generationMethod = 3,
            baseStats = Stats(scores[0], scores[1], scores[2], scores[3], scores[4], scores[5]),
            bonusChoices = Array.Empty<int>(),
            rollToken = token,
        };

        var token = roll["token"]!.GetValue<string>();
        var reversed = values.Reverse().ToArray();

        var ok = await client.PostAsJsonAsync("/characters/create", Draft(reversed, token));
        Assert.Equal(HttpStatusCode.Created, ok.StatusCode);

        var forged = await client.PostAsJsonAsync("/characters/create", Draft([18, 18, 18, 18, 18, 18], token));
        Assert.Equal(HttpStatusCode.BadRequest, forged.StatusCode);

        var noToken = await client.PostAsJsonAsync("/characters/create", Draft(values, null));
        Assert.Equal(HttpStatusCode.BadRequest, noToken.StatusCode);

        var (stranger, _) = await LoggedClientAsync();
        var stolen = await stranger.PostAsJsonAsync("/characters/create", Draft(values, token));
        Assert.Equal(HttpStatusCode.BadRequest, stolen.StatusCode);
    }

    [Fact]
    public async Task Справочники_содержат_структурные_поля()
    {
        var client = _factory.CreateClient();

        var halfElf = (await client.GetFromJsonAsync<JsonArray>("/races"))![1]!;
        var wizard = (await client.GetFromJsonAsync<JsonArray>("/classes"))![11]!;

        Assert.Equal(6, halfElf["asi"]![0]!["ability"]!.GetValue<int>());
        Assert.Null(halfElf["asi"]![1]!["ability"]);
        Assert.Equal(2, halfElf["asi"]![1]!["choices"]!.GetValue<int>());
        Assert.Equal(6, wizard["hitDie"]!.GetValue<int>());
        Assert.Equal([4, 5], wizard["savingThrows"]!.AsArray().Select(x => x!.GetValue<int>()));
    }

    [Fact]
    public async Task Старые_названия_классов_переименовываются_в_официальные()
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<DndContext>();
            await context.Classes.Where(x => x.Name == "Следопыт")
                .ExecuteUpdateAsync(x => x
                    .SetProperty(c => c.Name, "Рейнджер")
                    .SetProperty(c => c.Description, "старое описание"));
        }

        await _factory.Services.InitializeDbAsync();

        var classes = await _factory.CreateClient().GetFromJsonAsync<JsonArray>("/classes");
        var names = classes!.Select(x => x!["name"]!.GetValue<string>()).ToList();
        Assert.Contains("Следопыт", names);
        Assert.DoesNotContain("Рейнджер", names);
        Assert.Equal(12, names.Count);
        var ranger = classes!.Single(x => x!["name"]!.GetValue<string>() == "Следопыт")!;
        Assert.NotEqual("старое описание", ranger["description"]!.GetValue<string>());
    }
}
