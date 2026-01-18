using FCG.Catalog.Application.UseCases.Promotion.Create;
using FCG.Catalog.Application.UseCases.Promotion.Delete;
using FCG.Catalog.Application.UseCases.Promotion.Update;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController(IMediator mediator) : FcgCatalogBaseController(mediator)
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CreatePromotionOutput>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePromotionInput input)
        {
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Created(string.Empty, ApiResponse<CreatePromotionOutput>.SuccesResponse(output));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UpdatePromotionOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdatePromotionInput input, CancellationToken cancellationToken)
        {
            input.Id = id;
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Ok(ApiResponse<UpdatePromotionOutput>.SuccesResponse(output));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<DeletePromotionOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var input = new DeletePromotionInput { Id = id };
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Ok(ApiResponse<DeletePromotionOutput>.SuccesResponse(output));
        }
    }
}
