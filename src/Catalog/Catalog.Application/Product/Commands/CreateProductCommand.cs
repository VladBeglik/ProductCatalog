using MediatR;

namespace Catalog.Application.Product.Commands;

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
    public Task<string> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}