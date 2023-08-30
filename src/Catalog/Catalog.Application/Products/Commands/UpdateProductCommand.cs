using AutoMapper;
using Automapper.Infrastructure.Interfaces;
using Catalog.Application.Infrastructure;
using Catalog.Application.Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Products.Commands;

public class UpdateProductCommand : IRequest, IHaveCustomMapping
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? CategoryId { get; set; }
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public string NoteGeneral { get; set; } = null!;
    public string NoteSpecial { get; set; } = null!;
    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<UpdateProductCommand, Domain.Product>().ReverseMap();
    }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly ICatalogDbContext _ctx;
    private readonly IMapper _mapper;

    public UpdateProductCommandHandler(ICatalogDbContext ctx, IMapper mapper)
    {
        _ctx = ctx;
        _mapper = mapper;
    }

    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _ = await _ctx.Products.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken: cancellationToken)  ?? throw new CustomException();

        _mapper.Map<Domain.Product>(request);

        await _ctx.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}