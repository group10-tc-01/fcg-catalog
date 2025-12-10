using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Catalog.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcgCatalogBaseController(IMediator mediator) : ControllerBase
    {
        protected IMediator _mediator = mediator;
    }
}
