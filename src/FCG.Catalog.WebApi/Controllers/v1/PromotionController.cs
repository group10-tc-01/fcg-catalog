using System.Threading;
using System.Threading.Tasks;
using FCG.Catalog.Application.UseCases.Promotion.Create;
using FCG.Catalog.WebApi.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController(IMediator mediator) : FcgCatalogBaseController(mediator)
    {
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CreatePromotionResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePromotionRequest input)
        {
            var output = await _mediator.Send(input, CancellationToken.None).ConfigureAwait(false);
            return Created(string.Empty, ApiResponse<CreatePromotionResponse>.SuccesResponse(output));
        }
    }
}
