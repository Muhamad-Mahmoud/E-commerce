using System.Security.Claims;
using ECommerce.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("User not authenticated or ID claim missing.");

        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    var code when code.Contains("NotFound") => NotFound(result.Error),
                    var code when code.Contains("Unauthorized") => Forbid(),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(result.Value);
        }
    }
}
