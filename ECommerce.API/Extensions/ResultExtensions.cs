using ECommerce.API.Errors;
using ECommerce.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Extensions
{
    /// <summary>
    /// Extension methods for mapping Domain Results to API ActionResults.
    /// </summary>
    public static class ResultExtensions
    {
        /// <summary>
        /// Converts a <see cref="Result"/> to an <see cref="ActionResult"/>.
        /// </summary>
        /// <param name="result">The domain result.</param>
        /// <returns>A NoContentResult if successful, otherwise an ObjectResult with error details.</returns>
        public static ActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
            {
                return new NoContentResult();
            }

            return CreateErrorActionResult(result.Error);
        }

        /// <summary>
        /// Converts a <see cref="Result{T}"/> to an <see cref="ActionResult"/>.
        /// </summary>
        /// <typeparam name="T">The type of the result value.</typeparam>
        /// <param name="result">The domain result with a value.</param>
        /// <returns>An OkObjectResult if successful, otherwise an ObjectResult with error details.</returns>
        public static ActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
            {
                return new OkObjectResult(result.Value);
            }

            return CreateErrorActionResult(result.Error);
        }

        private static ActionResult CreateErrorActionResult(Error error)
        {
            var statusCode = error.StatusCode ?? 400;

            return new ObjectResult(new ApiErrorResponse(statusCode, error.Description ?? error.Code))
            {
                StatusCode = statusCode
            };
        }
    }
}
