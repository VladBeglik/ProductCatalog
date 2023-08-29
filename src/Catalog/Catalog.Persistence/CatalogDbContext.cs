using Catalog.Application.Infrastructure;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NodaTime;

namespace Catalog.Persistence;

public class CatalogDbContext : DbContext, ICatalogDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    #region Fields

    private readonly IClock _clock;
    private readonly ILogger<CatalogDbContext> _logger;

    #endregion
    
    #region Ctors

    public CatalogDbContext(ILogger<CatalogDbContext> logger, IClock clock)
    {
        _logger = logger;
        _clock = clock;
    }    
    
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options, ILogger<CatalogDbContext> logger, IClock clock) : base(options)
    {
        _logger = logger;
        _clock = clock;
    }

    #endregion

    #region Methods

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = new())
    {
        return Database.BeginTransactionAsync(token);
    }

    public IExecutionStrategy CreateExecutionStrategy()
    {
        return Database.CreateExecutionStrategy();
    }
    #endregion
}