using AutoMapper;
using Automapper.Infrastructure.Interfaces;
using Catalog.Application.Infrastructure;
using Catalog.Application.Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Category.Commands;

public class UpdateCategoryCommand : IRequest, IHaveCustomMapping
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string[]? ProductIds { get; set; }
    public void CreateMappings(Profile configuration)
    {
        configuration.CreateMap<UpdateCategoryCommand, Domain.Category>().ReverseMap();
    }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICatalogDbContext _ctx;

    public UpdateCategoryCommandHandler(ICatalogDbContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _ctx.Categories.FirstOrDefaultAsync(_ => _.Id == request.Id, cancellationToken: cancellationToken)  ?? throw new CustomException();
        var products = await _ctx.Products.Where(_ => request.ProductIds != null && request.ProductIds.Contains(_.Id)).ToListAsync(cancellationToken);
        
        category.Name = request.Name;
        category.Products = products;

        await _ctx.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}