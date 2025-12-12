using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    public class GamesController(IMediator mediator) : FcgCatalogBaseController(mediator)
    {
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RegisterGameOutput>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterGameInput input, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(input, cancellationToken);
            var response = ApiResponse<RegisterGameOutput>.SuccesResponse(result);
            return Created($"api/v1/games/{result.Id}", response);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<GetGameOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetGameInput(id), cancellationToken);
            var response = ApiResponse<GetGameOutput>.SuccesResponse(result);
            return Ok(response);
        }
    }
}
