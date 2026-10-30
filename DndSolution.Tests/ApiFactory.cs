using Data.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Tests;

/// <summary>
/// Поднимает API поверх отдельной тестовой БД.
/// Строка подключения берётся из переменной окружения DND_TEST_POSTGRES.
/// </summary>
public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("DND_TEST_POSTGRES")
        ?? "Host=localhost;Port=5432;Database=dnd_test;Username=dnd;Password=dnd";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.UseSetting("ConnectionStrings:Postgres", ConnectionString);
    }

    public async Task InitializeAsync()
    {
        // Чистая БД на каждый прогон, миграции и сидинг выполнит само приложение
        var options = new DbContextOptionsBuilder<DndContext>()
            .UseNpgsql(ConnectionString)
            .UseSnakeCaseNamingConvention()
            .Options;
        await using var context = new DndContext(options);
        await context.Database.EnsureDeletedAsync();

        _ = Server;
    }

    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
}
