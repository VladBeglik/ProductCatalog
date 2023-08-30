using Identity.Application.Infrastructure;
using Identity.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using NodaTime;

namespace Identity.API.Infrastructure
{
    public static class EFExtensions
    {
        public static void DatabaseMigrate(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
            var clock = scope.ServiceProvider.GetRequiredService<IClock>();
            context.Database.Migrate();
            Seed(context);

        }

        private static async void Seed(IdentityDbContext ctx)
        {
            // if (ctx.Books.Any())
            //     return;
            //
            // await ctx.Books.AddRangeAsync(Books());
            // await ctx.SaveChangesAsync();

        }


        #region private methods
        private static ILoggerFactory GetConsoleLoggerFactory()
        {
            return LoggerFactory.Create(builder =>
            {
                builder.AddConsole()
                    .AddFilter(level => level >= LogLevel.Warning);
            });
        }
        private static bool LogSqlToConsole = false;

        #endregion
    }
}
