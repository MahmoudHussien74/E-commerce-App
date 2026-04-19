namespace E_commerce.Application.Contracts.Authentication;

public record RegisterResponse(
    string UserId,
    string Name,
    string Email);
