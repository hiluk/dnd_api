using Data.Repositories.Di;
using DndSolution.Application.Services.Di;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureRepositories();
builder.Services.ConfigureServices();

builder.Services.AddDbContext<CharactersContext>(options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("CharactersContext"))
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();
