using DndSolution.Application.Abstractions;
using DndSolution.Application.Models;
using DndSolution.Neccessary;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Application.Services;

public static class ConfugureServicesExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IRaceService, RaceService>();
        services.AddTransient<IClassService, ClassService>();
        services.AddTransient<ICharacterService, CharacterService>();
        
        return services;
    }
    
    
}