using Catalog.Application.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Categories.Commands;

public class CreateCategoryCommand : IRequest<string>
{
    public string Name { get; set; } = null!;
    public string[]? ProductIds { get; set; }
}


public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, string>
{
    private readonly ICatalogDbContext _ctx;
    private readonly ICurrentUserService _currentUser;

    public CreateCategoryCommandHandler(ICatalogDbContext ctx, ICurrentUserService currentUser)
    {
        _ctx = ctx;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var id = _currentUser.UserId;
        
        
        var products = await _ctx.Products
            .Where(_ => request.ProductIds != null && request.ProductIds.Contains(_.Id))
            .ToListAsync(cancellationToken);
        
        var catalog = new Domain.Category
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Products = products
        };

        _ctx.Categories.Add(catalog);
        await _ctx.SaveChangesAsync(cancellationToken);
        return catalog.Id;
    }
}