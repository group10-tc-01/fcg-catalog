using FCG.Catalog.Application.UseCases.Games.Delete;
using FCG.Catalog.Application.UseCases.Games.Get;
using FCG.Catalog.Application.UseCases.Games.GetById;
using FCG.Catalog.Application.UseCases.Games.Purchase;
using FCG.Catalog.Application.UseCases.Games.Register;
using FCG.Catalog.Application.UseCases.Games.Update;
using FCG.Catalog.Domain.Models;
using FCG.Catalog.Domain.Services.Repositories;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [ExcludeFromCodeCoverage]
    public class GamesController(IMediator mediator, ICatalogLoggedUser catalogLoggedUser) : FcgCatalogBaseController(mediator)
    {
        [HttpPost]
       // [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<RegisterGameOutput>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterGameInput input, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(input, cancellationToken);
            var response = ApiResponse<RegisterGameOutput>.SuccesResponse(result);
            return Created($"api/v1/games/{result.Id}", response);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<GetGameOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromQuery] GetGameInput input)
        {
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Ok(ApiResponse<PagedListResponse<GetGameOutput>>.SuccesResponse(output));

        }
        [HttpPost("{id}/purchase")]
       // [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<PurchaseGameOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Purchase([FromRoute] Guid id)
        {
            var input = new PurchaseGameInput(id);
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);

            return Ok(ApiResponse<PurchaseGameOutput>.SuccesResponse(output));
        }

        [HttpPut("{id}")]
      //  [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UpdateGameOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateGameInput input, CancellationToken cancellationToken)
        {
            input.Id = id;
            var result = await _mediator.Send(input, cancellationToken).ConfigureAwait(false);
      
            return Ok(ApiResponse<UpdateGameOutput>.SuccesResponse(result));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<GetGameIdOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var input = new GetGameIdInput(id);
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Ok(ApiResponse<GetGameIdOutput>.SuccesResponse(output));
        }

        [HttpDelete("{id}")]
       // [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var input = new DeleteGameInput(id);
            await _mediator.Send(input, cancellationToken).ConfigureAwait(false);
            return NoContent();
        }


    }
}
