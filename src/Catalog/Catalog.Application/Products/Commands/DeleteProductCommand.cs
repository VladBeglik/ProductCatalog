using Catalog.Application.Infrastructure;
using Catalog.Application.Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Products.Commands;

public class DeleteProductCommand : IRequest
{
    public string Id { get; set; } = null!;
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly ICatalogDbContext _ctx;

    public DeleteProductCommandHandler(ICatalogDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _ctx.Products.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken: cancellationToken);

        if (product == default)
        {
            throw new CustomException();
        }

        _ctx.Products.Remove(product);
        await _ctx.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}