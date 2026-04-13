using E_commerce.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace E_commerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentsController(IPaymentService paymentService) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;


    [HttpPost("{basketId}")]
    public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentAsync(string basketId, [FromQuery] int? deliveryMethod)
    {
        var basket = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryMethod);

        if (basket == null) 
            return BadRequest(new ProblemDetails { Title = "Problem with your basket" });


        return Ok(basket);
    }
}