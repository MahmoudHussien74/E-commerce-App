using Microsoft.AspNetCore.Http;
namespace E_commerce.Infrastructure.Service;
public class AuthService(
    UserManager<User> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationDbContext dbContext) : IIdentityService
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => _userManager.Users.AnyAsync(x => x.Email == email, cancellationToken);

    public async Task<Result<ApplicationUser>> CreateUserAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var newUser = new User
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.Name
        };
        var result = await _userManager.CreateAsync(newUser, request.Password);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure<ApplicationUser>(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        if (await _roleManager.RoleExistsAsync(DefaultIdentityData.CustomerRoleName))
        {
            await _userManager.AddToRoleAsync(newUser, DefaultIdentityData.CustomerRoleName);
        }

        return Result.Success(Map(newUser));
    }

    public async Task<AuthenticatedIdentity?> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        var roles = await GetRolesAsync(user.Id, cancellationToken);
        var permissions = await GetPermissionsAsync(user.Id, cancellationToken);
        return new AuthenticatedIdentity(Map(user), roles, permissions);
    }
    public async Task<ApplicationUser?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        return user is null ? null : Map(user);
    }

    public async Task<IReadOnlyCollection<string>> GetRolesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
            return Array.Empty<string>();
        return [.. await _userManager.GetRolesAsync(user)];
    }

    public async Task<IReadOnlyCollection<string>> GetPermissionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await (from userRole in _dbContext.UserRoles
                      join rolePermission in _dbContext.RolePermissions on userRole.RoleId equals rolePermission.RoleId
                      join permission in _dbContext.Permissions on rolePermission.PermissionId equals permission.Id
                      where userRole.UserId == userId
                      select permission.Name)
            .Distinct()
            .ToListAsync(cancellationToken);
    }
    private static ApplicationUser Map(User user)
        => new(user.Id, user.Email ?? string.Empty, user.FirstName, user.LastName, user.IsDisabled);
}