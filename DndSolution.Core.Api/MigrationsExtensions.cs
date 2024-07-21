using Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Api;

public static class MigrationsExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        using DndContext context = scope.ServiceProvider.GetRequiredService<DndContext>();
        
        context.Database.Migrate();
    }
}