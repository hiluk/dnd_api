﻿using System.Text;
using Data.Abstractions;
using Data.Repositories.Migrations;
using DndSolution.Neccessary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Data.Repositories;

/// <summary>
/// Внедрение зависимостей слоя репозиториев
/// </summary>
public static class DIExtensions
{
    public static void ConfigureDb(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("PostgresSQL").Value;

        services
            .AddEntityFrameworkNpgsql()
            .AddEntityFrameworkNamingConventions();
        
        services.AddDbContext<DndContext>(options =>
            {
                options
                    .UseNpgsql(connectionString)
                    .UseSnakeCaseNamingConvention()
                    .EnableSensitiveDataLogging();
            }
            , ServiceLifetime.Scoped);
        
        services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
        services.AddTransient<IRacesRepository, RacesRepository>();
        services.AddTransient<IClassesRepository, ClassesRepository>();
        services.AddTransient<ICharactersRepository, CharactersRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        
    }
    
    public static void AddApiAuthenication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>();
        
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtOptions!.SecretKey))
                };
            });
        
        

        services.AddAuthorization();
    }
}