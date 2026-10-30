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

    private static object NewCharacter(string name) => new
    {
        name,
        level = 2,
        characterClass = 3,
        characterRace = 2,
        characterStats = new { strength = 10, dexterity = 15, constitution = 12, intelligence = 11, wisdom = 13, charisma = 14 },
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
        var stats = character!["characterStats"]!;
        Assert.Equal("Леголас", character["name"]!.GetValue<string>());
        Assert.Equal(15, stats["dexterity"]!.GetValue<int>());
        Assert.Equal(12, stats["constitution"]!.GetValue<int>());

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
            level = 99,
            characterClass = 42,
            characterRace = 1,
            characterStats = new { strength = 100 },
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
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
