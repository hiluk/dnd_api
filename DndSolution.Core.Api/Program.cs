using Core.Api;
using Data.Repositories;
using DndSolution.Application.Services;
using DndSolution.Neccessary;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureDb(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.AddApiAuthenication(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
await using (var context = scope.ServiceProvider.GetRequiredService<DndContext>())
{
    context.Database.Migrate();
    context.Database.EnsureCreated();
}



app.UseHttpsRedirection();
app.MapControllers();

app.Run();
