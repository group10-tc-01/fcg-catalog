using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace FCG.Catalog.WebApi.Controllers.v1
{
    [ExcludeFromCodeCoverage]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class FcgCatalogBaseController(IMediator mediator) : ControllerBase
    {
        protected IMediator _mediator = mediator;
    }
}
