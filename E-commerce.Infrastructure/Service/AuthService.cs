using Microsoft.AspNetCore.Http;

namespace E_commerce.Infrastructure.Service;

public class AuthService(UserManager<User> userManager, IJwtProvider jwtProvider) : IAuthService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);

        var result =await _userManager.CheckPasswordAsync(user,password:request.Password);

        if (result)
        {
            var (token, expire) = _jwtProvider.GenerateToken(user);


            var response = new AuthResponse(user.Id, user.Name, user.Email!, token, expire);

            return Result.Success(response);

        }

        return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
    }

    public async Task<Result> RegisterAsync(RegisterRequest request)
    {
        var userIsExsist = await _userManager.Users.AnyAsync(x => x.Email == request.Email);

        if (userIsExsist)
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

       
        return Result.Success();

    }

   
}
