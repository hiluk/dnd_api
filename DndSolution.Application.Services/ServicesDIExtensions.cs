using DndSolution.Application.Abstractions;
using DndSolution.Neccessary;
using Microsoft.Extensions.DependencyInjection;

namespace DndSolution.Application.Services;

public static class ServicesDIExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        services.AddTransient<IRaceService, RaceService>();
        services.AddTransient<IJwtOptions, JwtOptions>();
        services.AddTransient<IJwtProvider, JwtProvider>();
        services.AddTransient<IPasswordHasher, PasswordHasher>();
        services.AddTransient<IClassService, ClassService>();
        services.AddTransient<ICharacterService, CharacterService>();
        services.AddTransient<IUserService, UserService>();
        
        
        return services;
    }
    
    
}