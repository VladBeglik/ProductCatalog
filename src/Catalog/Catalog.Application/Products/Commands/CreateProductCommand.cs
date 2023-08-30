using Catalog.Application.Infrastructure;
using MediatR;

namespace Catalog.Application.Products.Commands;

public class CreateProductCommand : IRequest<string>
{
    public string Name { get; set; } = null!;
    public string? CategoryId { get; set; }
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string NoteGeneral { get; set; } = null!;
    public string NoteSpecial { get; set; } = null!;
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, string>
{
    private readonly ICatalogDbContext _ctx;

    public CreateProductCommandHandler(ICatalogDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Domain.Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = request.Name,
            Description = request.Description,
            NoteSpecial = request.NoteSpecial,
            CategoryId = request.CategoryId,
            NoteGeneral = request.NoteGeneral,
            Price = request.Price
        };

        _ctx.Products.Add(product);

        await _ctx.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}