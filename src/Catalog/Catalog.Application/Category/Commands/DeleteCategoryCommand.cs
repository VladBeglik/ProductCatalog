using Catalog.Application.Infrastructure;
using Catalog.Application.Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Category.Commands;

public class DeleteCategoryCommand : IRequest
{
    public string Id { get; set; } = null!;
}


public class DeleteCatalogCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    
    private readonly ICatalogDbContext _ctx;

    public DeleteCatalogCommandHandler(ICatalogDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var catalog = await _ctx.Categories.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken: cancellationToken);

        if (catalog == default)
        {
            throw new CustomException();
        }

        _ctx.Categories.Remove(catalog);
        await _ctx.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }
}