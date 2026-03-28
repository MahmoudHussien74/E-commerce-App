using E_commerce.Core.Contracts.Order;
using E_commerce.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commerce.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderRequest orderRequest)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        var result = await _orderService.CreateOrdersAsync(orderRequest, email);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        var result = await _orderService.GetAllOrdersForUserAsync(email);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrderById([FromRoute] int id)
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        if (email is null) return Unauthorized();

        var result = await _orderService.GetOrderByIdAsync(id, email);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("deliveryMethods")]
    public async Task<IActionResult> GetDeliveryMethods()
    {
        var result = await _orderService.GetDeliveryMethodAsync();

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
