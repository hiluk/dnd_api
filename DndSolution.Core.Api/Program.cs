using Core.Api;
using Core.Api.Swagger;
using Data.Repositories;
using DndSolution.Application.Models;
using DndSolution.Application.Services;
using DndSolution.Neccessary;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.ConfigureSwagger();

services.ConfigureDb(builder.Configuration);
services.ConfigureServices();

services.AddAuthorization();
services.AddAuthentication().AddBearerToken(IdentityConstants.BearerScheme);

services.AddIdentityCore<User>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<DndContext>()
    .AddApiEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.ApplyMigrations();
}

using var scope = app.Services.CreateScope();
await using (var context = scope.ServiceProvider.GetRequiredService<DndContext>())
{
    context.Database.Migrate();
    context.Database.EnsureCreated();
}

app.MapCustomIdentityApi<User>();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
