namespace E_commerce.Core.Entities;

public sealed class RefreshToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresOnUtc { get; set; }
    public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedOnUtc { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresOnUtc;
    public bool IsActive => RevokedOnUtc is null && !IsExpired;
}

public static class DefultRoles
{
    public const string Admin = "Admin";
    public const string Customer = "Customer";

    public const string AdminRoleId = "role-admin";
    public const string CustomerRoleId = "role-member";

    public const string AdminRoleConcurrencyStamp = "9A0EAFA5-FE54-4C09-9DAA-AE8D468D3D41";
    public const string CustomerRoleConcurrencyStamp = "9CC51962-7A32-41C1-8532-6E6624FF63D2";
}

public static class DefultUsers
{
    public const string AdminId = "user-admin";
    public const string AdminEmail = "admin@ecommerce.local";
    public const string AdminSecurityStamp = "4B71E0D0-3191-417A-9E71-D6B887838489";
    public const string AdminConcurrencyStamp = "8BB27CF0-A57F-4D52-B469-5700412D6D96";
    public const string AdminPassword = "AQAAAAIAAYagAAAAEOkKwQwZyB40kkM3gc6L4tMDOvBI7g4gTPV7sYcA8Y2e6x6fT8dVx7B4m2Mdn3S6WA==";
}
