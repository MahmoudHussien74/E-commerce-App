using Microsoft.AspNetCore.Http;

namespace E_commerce.Infrastructure.Service;

public class AuthService(UserManager<User> userManager) : IAuthService
{
    private readonly UserManager<User> _userManager = userManager;

    public Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {

        var userIsExsist = await _userManager.Users.AnyAsync(x=>x.Email == request.Email);

        if(userIsExsist)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);


        var newuser = new User
        {
            Email = request.Email,
            UserName = request.Email,
            Name = request.Name,

        };

        var result = await _userManager.CreateAsync(newuser, request.Password);

        if (!result.Succeeded)
        {

            var error = result.Errors.First();

            return Result.Failure<AuthResponse>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        var response = new AuthResponse(newuser.Id, newuser.Email!, newuser.Name);

        return Result.Success(response);

    }
}
