using Catalog.Application.Products.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

public class ProductController : MediatrController
{
    /// <summary>
    /// Создать продукт
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="200"></response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<string>> Create(CreateProductCommand request, CancellationToken token)
    {
        var id = await Mediator.Send(request, token);
        return Ok(id);
    }

    /// <summary>
    /// Удалить продукт
    /// </summary>
    /// <param name="id"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="204"></response>
    [HttpDelete]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(string id, CancellationToken token)
    {
        await Mediator.Send(new DeleteProductCommand { Id = id }, token);
        return NoContent();
    }

    /// <summary>
    /// Обновить продукт
    /// </summary>
    /// <param name="request"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    /// <response code="204">Обновляет клуб</response>
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update(string id, UpdateProductCommand request, CancellationToken token)
    {
        request.Id = id;
        await Mediator.Send(request, token);
        return NoContent();
    }
}