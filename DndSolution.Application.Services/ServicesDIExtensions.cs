using DndSolution.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Application.Services;

public static class ServicesDIExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<ICharacterService, CharacterService>();
        
        return services;
    }
}