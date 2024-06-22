using Data.Repositories;
using DndSolution.Application.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

await using var context = app.Services.CreateScope().ServiceProvider.GetRequiredService<DndContext>();
context.Database.EnsureCreated();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
