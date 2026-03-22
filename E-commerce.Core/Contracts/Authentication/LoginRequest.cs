namespace E_commerce.Core.Contracts.Authentication;

public record LoginRequest(
       string Email,
       string Password
);
