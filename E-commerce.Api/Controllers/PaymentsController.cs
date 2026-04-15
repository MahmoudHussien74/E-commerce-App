// UPDATED WITH TOPROBLEM
using E_commerce.Core.Contracts.Basket;
using E_commerce.Core.Interfaces;
using E_commerce.Api.Abstraction;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace E_commerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentService paymentService,
    ILogger<PaymentsController> logger) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PaymentsController> _logger = logger;

    [HttpPost("{basketId}")]
    [Authorize]
    public async Task<ActionResult<CustomerBasketResponse>> CreateOrUpdatePaymentAsync(string basketId, [FromQuery] int? deliveryMethod)
    {
        var result = await _paymentService.CreateOrUpdatePaymentAsync(basketId, deliveryMethod);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    [HttpPost("webhook")]
    public async Task<ActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Missing Stripe-Signature header.");
            return BadRequest();
        }

        var result = await _paymentService.ProcessWebhookAsync(json, signature!);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}