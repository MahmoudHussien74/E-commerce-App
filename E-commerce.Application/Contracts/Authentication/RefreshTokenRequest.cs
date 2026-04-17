namespace E_commerce.Application.Contracts.Authentication;

public record RefreshTokenRequest(string AccessToken, string RefreshToken);
