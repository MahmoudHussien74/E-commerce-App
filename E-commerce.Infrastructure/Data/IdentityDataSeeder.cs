using E_commerce.Core.Entities.Authorization;
using E_commerce.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace E_commerce.Infrastructure.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Seed Permissions
        var existingPermissions = await dbContext.Permissions.ToListAsync();
        foreach (var (name, description) in PermissionNames.All)
        {
            if (!existingPermissions.Any(p => p.Name == name))
            {
                dbContext.Permissions.Add(new Permission
                {
                    Name = name,
                    Description = description
                });
            }
        }

        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }

        // Refresh permissions list
        var allPermissions = await dbContext.Permissions.ToListAsync();

        // 2. Link Permissions to Roles
        // Admin: All Permissions
        await AssignPermissionsToRole(dbContext, DefaultIdentityData.AdminRoleId, allPermissions.Select(p => p.Id));

        // Customer: Basic Permissions
        var customerPermissions = allPermissions.Where(p => 
            p.Name == PermissionNames.ProductsRead ||
            p.Name == PermissionNames.CategoriesRead ||
            p.Name == PermissionNames.OrdersRead ||
            p.Name == PermissionNames.OrdersCreate ||
            p.Name == PermissionNames.BasketWrite ||
            p.Name == PermissionNames.PaymentsCreate ||
            p.Name == PermissionNames.DeliveryMethodsRead
        ).Select(p => p.Id);

        await AssignPermissionsToRole(dbContext, DefaultIdentityData.CustomerRoleId, customerPermissions);

        if (dbContext.ChangeTracker.HasChanges())
        {
            await dbContext.SaveChangesAsync();
        }
    }

    private static async Task AssignPermissionsToRole(ApplicationDbContext dbContext, string roleId, IEnumerable<int> permissionIds)
    {
        var existingRolePermissions = await dbContext.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync();

        foreach (var permissionId in permissionIds)
        {
            if (!existingRolePermissions.Contains(permissionId))
            {
                dbContext.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                });
            }
        }
    }
}
