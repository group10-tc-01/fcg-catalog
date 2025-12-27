using FCG.Catalog.Application.UseCases.Libraries.Get;
using FCG.Catalog.Domain.Exception;
using FCG.Catalog.Domain.Services.Repositories;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController(IMediator mediator, ICatalogLoggedUser catalogLoggedUser) : FcgCatalogBaseController(mediator)
    {
        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken = default)
        {
            var loggedUser = await catalogLoggedUser.GetLoggedUserAsync();
            if (loggedUser == null || loggedUser.Id == Guid.Empty)
                throw new UnauthorizedException("User not authenticated.");

            var query = new GetLibraryByUserIdQuery(loggedUser.Id);

            var output = await _mediator.Send(query, cancellationToken);
            return Ok(ApiResponse<GetLibraryByUserIdResponse>.SuccesResponse(output));
        }
    }
}
