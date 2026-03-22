using E_commerce.Core.Common;
using E_commerce.Core.Contracts.Authentication;

namespace E_commerce.Core.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
}
