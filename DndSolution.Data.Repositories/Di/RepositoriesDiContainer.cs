using Data.Abstractions;
using DndSolution.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Repositories.Di;

public static class RepositoriesDiContainer
{

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        services.AddTransient<ICharactersRepository, CharactersRepository>();
        return services;
    }
}