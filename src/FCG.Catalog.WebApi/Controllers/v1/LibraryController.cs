using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController(IMediator mediator) : FcgCatalogBaseController(mediator)
    {
        [HttpGet]
        public async Task Get()
        {
            var output = _mediator.Send()
        }
    }
}
