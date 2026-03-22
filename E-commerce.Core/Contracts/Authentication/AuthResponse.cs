namespace E_commerce.Core.Contracts.Authentication;

public record AuthResponse(
      string Id,
      string Email,
      string Name
      //string Token,
      //int ExpiresIn,
      //string refreshToken,
      //DateTime expireationRefreshToken
 );
