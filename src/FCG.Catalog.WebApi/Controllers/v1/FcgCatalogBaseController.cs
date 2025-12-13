using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

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
