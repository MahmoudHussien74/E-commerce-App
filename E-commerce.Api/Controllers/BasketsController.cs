using Microsoft.AspNetCore.Authorization;
using E_commerce.Infrastructure.Authentication.Permissions;
using E_commerce.Application.Abstractions.Authorization;

namespace E_commerce.Api.Controllers
{
    /// <summary>
    /// Manages the shopping basket for the authenticated user.
    /// All operations are automatically scoped to the current user's basket.
    /// </summary>
    [HasPermission(PermissionPolicyNames.BasketWrite)]
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController(IBasketService basketService) : ControllerBase
    {
        private readonly IBasketService _basketService = basketService;

        /// <summary>
        /// Get the current user's shopping basket including all items and their details.
        /// </summary>
        /// <response code="200">Returns the basket with all its items.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No basket found for the current user.</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get()
        {
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _basketService.GetBasketAsync(userId, HttpContext.RequestAborted);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        /// <summary>
        /// Add or update items in the current user's basket.
        /// If a basket already exists it will be updated; otherwise a new one will be created.
        /// </summary>
        /// <param name="basket">The basket payload containing items to add or update.</param>
        /// <response code="200">Basket updated successfully. Returns the updated basket.</response>
        /// <response code="400">Validation failed (e.g., invalid product IDs or quantities).</response>
        /// <response code="401">User is not authenticated.</response>
        [HttpPost]
        [EnableRateLimiting("basketLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Add([FromBody] BasketUpdateRequest basket)
        {
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _basketService.UpdateBasketAsync(userId, basket, HttpContext.RequestAborted);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }

        /// <summary>
        /// Clear and delete the current user's basket.
        /// </summary>
        /// <response code="204">Basket deleted successfully.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">No basket found for the current user.</response>
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete()
        {
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _basketService.DeleteBasketAsync(userId, HttpContext.RequestAborted);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }

        /// <summary>
        /// Delete a single item from the current user's basket.
        /// </summary>
        /// <param name="itemId">The ID of the item to remove.</param>
        /// <response code="200">Item deleted and returns updated basket.</response>
        /// <response code="401">User is not authenticated.</response>
        /// <response code="404">Basket or item not found.</response>
        [HttpDelete("{itemId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteItem([FromRoute] int itemId)
        {
            var userId = User.GetUserId();
            if (userId is null) return Unauthorized();

            var result = await _basketService.DeleteItemAsync(userId, itemId, HttpContext.RequestAborted);

            return result.IsSuccess
                ? Ok(result.Value)
                : result.ToProblem();
        }
    }
}
