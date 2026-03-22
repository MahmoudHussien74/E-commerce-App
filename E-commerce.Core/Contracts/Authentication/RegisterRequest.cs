namespace E_commerce.Core.Contracts.Authentication;

public record RegisterRequest(
    string Name,
    string Email,
    string Password
);
