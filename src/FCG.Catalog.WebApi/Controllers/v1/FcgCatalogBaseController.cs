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
        protected Guid? UserId
        {
            get
            {
                if (User?.Identity?.IsAuthenticated != true)
                    return null;

                var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub");

                return Guid.TryParse(claim?.Value, out var id) ? id : null;
            }
        }
        protected Guid CurrentUserId
        {
            get
            {
                var id = UserId;
                if (id is null)
                {
                    throw new UnauthorizedAccessException("User is not authenticated or ID is invalid.");
                }
                return id.Value;
            }
        }
    }
}
