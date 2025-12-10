using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController(IMediator mediator) : FcgCatalogBaseController(mediator)
    {
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterGameInput input, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(input, cancellationToken);
            var response = ApiResponse<RegisterGameOutput>.SuccesResponse(result);
            return Created($"api/games/{result.Id}", response);
        }
    }
}
