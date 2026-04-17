namespace E_commerce.Api.Controllers;

/// <summary>
/// Handles user registration, login, and token refresh.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService,ILogger<AuthController> logger) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    /// <summary>
    /// Register a new user account.
    /// </summary>
    /// <param name="request">Registration data (name, email, password).</param>
    /// <response code="201">User created successfully. Returns the new user's ID and basic info.</response>
    /// <response code="400">Validation failed or email already in use.</response>
    [HttpPost("register")]
    [EnableRateLimiting("ipLimiter")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request, HttpContext.RequestAborted);

        return result.IsSuccess
            ? CreatedAtAction(nameof(Register), new { id = result.Value.UserId }, result.Value)
            : result.ToProblem();
    }

    /// <summary>
    /// Authenticate a user and obtain a JWT access token.
    /// </summary>
    /// <param name="request">Login credentials (email and password).</param>
    /// <response code="200">Login successful. Returns a signed JWT token and refresh token.</response>
    /// <response code="400">Invalid request body.</response>
    /// <response code="401">Email or password is incorrect.</response>
    [HttpPost("login")]
    [EnableRateLimiting("ipLimiter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email: {Email}", request.Email);
        var result = await _authService.LoginAsync(request, HttpContext.RequestAborted);

        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    /// <summary>
    /// Exchange a valid refresh token for a new JWT access token.
    /// </summary>
    /// <param name="request">The current refresh token.</param>
    /// <response code="200">Token refreshed. Returns a new JWT and refresh token pair.</response>
    /// <response code="400">Refresh token is missing or malformed.</response>
    /// <response code="401">Refresh token has expired or is invalid.</response>
    [HttpPost("refresh-token")]
    [EnableRateLimiting("ipLimiter")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var result = await _authService.RefreshTokenAsync(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
