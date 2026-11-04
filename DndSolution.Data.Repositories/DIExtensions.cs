using Data.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Data.Repositories;

/// <summary>
/// Внедрение зависимостей слоя репозиториев
/// </summary>
public static class DIExtensions
{
    public static void ConfigureDb(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Не задана строка подключения ConnectionStrings:Postgres");

        services.AddDbContext<DndContext>(options =>
        {
            options
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();

            if (environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
            }
        });

        services.AddScoped<IRacesRepository, RacesRepository>();
        services.AddScoped<IClassesRepository, ClassesRepository>();
        services.AddScoped<ICharactersRepository, CharactersRepository>();
        services.AddScoped<IBackgroundsRepository, BackgroundsRepository>();
        services.AddScoped<ISessionRepository, SessionRepository>();

        // Картинки персонажей лежат на диске. В Docker папку стоит вынести в том
        var avatarsPath = configuration["Avatars:Path"] is { Length: > 0 } path
            ? path
            : Path.Combine(environment.ContentRootPath, "data", "avatars");
        services.AddSingleton<IAvatarStorage>(new FileAvatarStorage(avatarsPath));
    }
}
