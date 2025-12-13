using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.Purchase;
using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Domain.Services.Repositories;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [ExcludeFromCodeCoverage]
    public class GamesController(IMediator mediator, ICatalogLoggedUser catalogLoggedUser) : FcgCatalogBaseController(mediator)
    {
        private readonly ICatalogLoggedUser _catalogLoggedUser = catalogLoggedUser;

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<RegisterGameOutput>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterGameInput input, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(input, cancellationToken);
            var response = ApiResponse<RegisterGameOutput>.SuccesResponse(result);
            return Created($"api/v1/games/{result.Id}", response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<GetGameOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] GetGameInput input)
        {
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Ok(ApiResponse<PagedListResponse<GetGameOutput>>.SuccesResponse(output));

        }
        [HttpPost("{id}/purchase")]
        [ProducesResponseType(typeof(ApiResponse<PurchaseGameOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Purchase([FromRoute] Guid id)
        {
            var logged = await _catalogLoggedUser.GetLoggedUserAsync();
            var userId = logged?.Id ?? Guid.Empty;

            var input = new PurchaseGameInput(id, userId);
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Ok(ApiResponse<PurchaseGameOutput>.SuccesResponse(output));
        }
    }
}
