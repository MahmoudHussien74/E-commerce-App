using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Authentication;

namespace E_commerce.Core.Entities;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result> RegisterAsync(RegisterRequest request);
}
