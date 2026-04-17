using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace E_commerce.Api.Controllers;

/// <summary>
/// Handles Stripe payment intent creation and webhook event processing.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PaymentsController(
    IPaymentService paymentService,
    ILogger<PaymentsController> logger) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;
    private readonly ILogger<PaymentsController> _logger = logger;

    /// <summary>
    /// Create or update a Stripe PaymentIntent for the current user's basket.
    /// Call this before the checkout step to obtain a <c>clientSecret</c> for the Stripe SDK.
    /// </summary>
    /// <param name="deliveryMethod">Optional delivery method ID to include the delivery cost in the total.</param>
    /// <response code="200">PaymentIntent created/updated. Returns the basket with the Stripe client secret.</response>
    /// <response code="400">Basket is empty or delivery method is invalid.</response>
    /// <response code="401">User is not authenticated.</response>
    [HttpPost]
    [Authorize]
    [EnableRateLimiting("userLimiter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<CustomerBasketResponse>> CreateOrUpdatePaymentAsync([FromQuery] int? deliveryMethod)
    {
        var userId = User.GetUserId();
        if (userId is null) return Unauthorized();

        var result = await _paymentService.CreateOrUpdatePaymentAsync(userId, deliveryMethod, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok(result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Stripe webhook endpoint — receives and processes payment lifecycle events (e.g., payment_intent.succeeded).
    /// This endpoint is called by Stripe only and must NOT be called manually.
    /// Validates the request using the <c>Stripe-Signature</c> header.
    /// </summary>
    /// <response code="200">Webhook event processed successfully.</response>
    /// <response code="400">Missing or invalid Stripe-Signature header.</response>
    [HttpPost("webhook")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> StripeWebhook()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        var signature = Request.Headers["Stripe-Signature"];

        if (string.IsNullOrEmpty(signature))
        {
            _logger.LogWarning("Missing Stripe-Signature header.");
            return BadRequest();
        }

        var result = await _paymentService.ProcessWebhookAsync(json, signature!, HttpContext.RequestAborted);

        return result.IsSuccess
            ? Ok()
            : result.ToProblem();
    }
}
