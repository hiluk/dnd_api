using Core.Api;
using Data.Repositories;
using DndSolution.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureServices();

builder.Services.AddAuth();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DndContext>();
context.Database.EnsureCreated();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
