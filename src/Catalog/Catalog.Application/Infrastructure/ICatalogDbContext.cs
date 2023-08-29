using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Catalog.Application.Infrastructure;

public interface ICatalogDbContext
{
    
    DbSet<Domain.Product> Products { get; set; }
    DbSet<Domain.Category> Categories { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token = default);
    IExecutionStrategy CreateExecutionStrategy();
}