using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SharedCore.Infrastructure.EntityFramework;

public static class EfExtensions
{
    public static async Task MigrateDbContext<TDbContext>(this IServiceProvider sp)
        where TDbContext : DbContext
    {
        await using var scope = sp.CreateAsyncScope();
        await using var ctx = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await ctx.Database.MigrateAsync();
    }
}