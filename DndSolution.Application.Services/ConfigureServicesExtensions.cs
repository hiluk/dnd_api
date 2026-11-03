using DndSolution.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Application.Services;

public static class ConfigureServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IRaceService, RaceService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ICharacterService, CharacterService>();
        services.AddSingleton<IAbilityRollService, AbilityRollService>();

        return services;
    }
}
