using System.Security.Claims;
using ECommerce.API.Extensions;
using ECommerce.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    /// <summary>
    /// Base API controller providing common functionality like user context and result handling.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Gets the current authenticated user's ID.
        /// </summary>
        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("User not authenticated or ID claim missing.");

        /// <summary>
        /// Handles the <see cref="Result{T}"/> by converting it to an <see cref="ActionResult"/>.
        /// </summary>
        protected ActionResult HandleResult<T>(Result<T> result) => result.ToActionResult();

        /// <summary>
        /// Handles the <see cref="Result"/> by converting it to an <see cref="ActionResult"/>.
        /// </summary>
        protected ActionResult HandleResult(Result result) => result.ToActionResult();
    }
}

