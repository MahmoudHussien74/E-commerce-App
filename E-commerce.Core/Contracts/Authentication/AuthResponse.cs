namespace E_commerce.Core.Contracts.Authentication;

public record AuthResponse(
      string Id,
      string Name,
      string Email,
      string Token,
      int expireMinute
 );
