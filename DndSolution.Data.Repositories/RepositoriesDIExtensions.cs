﻿using Data.Abstractions;
using Data.Repositories.Migrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Repositories;

/// <summary>
/// Внедрение зависимостей слоя репозиториев
/// </summary>
public static class RepositoriesDIExtensions
{
    public static IServiceCollection ConfigureRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("PostgresSQL").Value;

        services.AddEntityFrameworkNpgsql().AddEntityFrameworkNamingConventions();
        
        services.AddDbContext<DndContext>(options =>
            {
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging();
            }
            , ServiceLifetime.Scoped);

        services.AddTransient<IRacesRepository, RacesRepository>();
        services.AddTransient<IClassesRepository, ClassesRepository>();
        services.AddTransient<ICharactersRepository, CharactersRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        
        return services;
    }
}