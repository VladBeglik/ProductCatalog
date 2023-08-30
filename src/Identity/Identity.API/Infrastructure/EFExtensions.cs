using EntityFrameworkCore.SqlServer.NodaTime.Extensions;
using Identity.Application.Infrastructure;
using Identity.Persistence;
using Microsoft.EntityFrameworkCore;
using NodaTime;

namespace Identity.API.Infrastructure
{
    public static class EFExtensions
    {
        public static IServiceCollection AddProjectDbContexts(this IServiceCollection services, IConfiguration configuration)
        {
            var migrationsAssembly = typeof(IdentityDbContext).Assembly;

            var connectionString = configuration.GetConnectionString("SqlServer");

            services.AddDbContext<IdentityDbContext>(options =>
            {
                options.UseSqlServer(connectionString, o =>
                {
                    o.MigrationsAssembly(migrationsAssembly.GetName().Name);
                    o.EnableRetryOnFailure(15);
                    o.UseNodaTime();
                });

                if (!LogSqlToConsole) return;

                //options.EnableSensitiveDataLogging();
                //options.UseLoggerFactory(GetConsoleLoggerFactory());
            }
            );

            services.AddScoped<IIdentityDbContext, IdentityDbContext>();

            return services;
        }

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
