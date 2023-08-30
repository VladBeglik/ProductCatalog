using System.Reflection;
using System.Reflection.Emit;
using Identity.Application.Infrastructure;
using Identity.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using NodaTime;

namespace Identity.Persistence;

public class IdentityDbContext : IdentityDbContext<User>, IIdentityDbContext
{
    #region Fields
    private readonly IClock _clock;
    #endregion

    public DbSet<User> Users { get; set; }

    #region ctors
    public IdentityDbContext()
    {

    }

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options, IClock clock)
        : base(options)
    {
        _clock = clock;
    }

    #endregion

    #region Methods

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder
            .ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = default)
    {
        return Database.BeginTransactionAsync(token);
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return Database.CreateExecutionStrategy();
    }
    #endregion
}