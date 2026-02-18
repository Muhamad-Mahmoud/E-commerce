using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new UnauthorizedAccessException("User not authenticated or ID claim missing.");
    }
}
