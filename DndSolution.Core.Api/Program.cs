using Core.Api;
using Core.Api.Swagger;
using Data.Repositories;
using Data.Entities;
using DndSolution.Application.Services;
using Microsoft.AspNetCore.Identity;

const string DevCorsPolicy = "DevCors";

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddProblemDetails();
services.AddExceptionHandler<CharacterRulesExceptionHandler>();
services.AddEndpointsApiExplorer();
services.ConfigureSwagger();

services.ConfigureDb(builder.Configuration, builder.Environment);
services.ConfigureServices();

services.AddAuthorization();
services.AddAuthentication(IdentityConstants.BearerScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

services.AddIdentityCore<UserEntity>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<DndContext>()
    .AddErrorDescriber<RussianIdentityErrorDescriber>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

// Flutter web в режиме разработки запускается на случайном порту
services.AddCors(options => options.AddPolicy(DevCorsPolicy, policy =>
    policy.SetIsOriginAllowed(origin => new Uri(origin).IsLoopback)
        .AllowAnyHeader()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(DevCorsPolicy);
}
else
{
    app.UseHttpsRedirection();
}

await app.Services.InitializeDbAsync();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Точка входа (нужна для интеграционных тестов)
/// </summary>
public partial class Program;
