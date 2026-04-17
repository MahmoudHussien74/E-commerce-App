using E_commerce.Application.Abstractions.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace E_commerce.Api.Controllers;

/// <summary>
/// Manages orders for the authenticated user — creating, listing, and retrieving orders.
/// All endpoints require authentication.
/// </summary>
[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    /// <summary>
    /// Place a new order using the current user's basket.
    /// </summary>
    /// <param name="orderRequest">Order details including delivery method and shipping address.</param>
    /// <response code="201">Order placed successfully. Returns the created order.</response>
    /// <response code="400">Validation failed or basket is empty.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [EnableRateLimiting("userLimiter")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
    {
        var userId = User.GetUserId();
        var email = User.GetEmailAddress();
        if (userId is null || email is null) return Unauthorized();

        var result = await _orderService.CreateOrderAsync(userId, email, orderRequest, HttpContext.RequestAborted);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetOrderById), new { id = result.Value.Id }, result.Value)
            : result.ToProblem();
    }
    /// <summary>
    /// Get all orders belonging to the authenticated user.
    /// </summary>
    /// <response code="200">Returns the list of orders for the current user.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _orderService.GetAllOrdersForUserAsync(userId, HttpContext.RequestAborted);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Get a specific order by its ID. Only the owner can access their own orders.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <response code="200">Returns the order details.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Order not found or does not belong to the current user.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetOrderById([FromRoute] int id)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _orderService.GetOrderByIdAsync(id, userId, HttpContext.RequestAborted);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Update the status of a specific order.
    /// </summary>
    /// <param name="id">The order ID.</param>
    /// <param name="request">The new status request.</param>
    /// <response code="200">Order status updated successfully.</response>
    /// <response code="400">Invalid status provided.</response>
    /// <response code="401">User is not authenticated.</response>
    /// <response code="404">Order not found.</response>
    [HttpPatch("{id}/status")]
    [Authorize(Policy = PermissionPolicyNames.OrdersUpdate)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, [FromBody] UpdateOrderStatusRequest request)
    {
        var result = await _orderService.UpdateOrderStatusAsync(id, request.Status, HttpContext.RequestAborted);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
