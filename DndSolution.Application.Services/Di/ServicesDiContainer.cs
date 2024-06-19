using Data.Abstractions;
using Data.Repositories;
using DndSolution.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Application.Services.Di;

public static class ServicesDiContainer
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICharactersService, CharactersService>();
        return services;
    }
}