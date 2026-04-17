using E_commerce.Core.Entities.Authorization;

namespace E_commerce.Application.Abstractions.Authorization;

public static class PermissionPolicyNames
{
    public const string ProductsRead = PermissionNames.ProductsRead;
    public const string ProductsCreate = PermissionNames.ProductsCreate;
    public const string ProductsUpdate = PermissionNames.ProductsUpdate;
    public const string ProductsDelete = PermissionNames.ProductsDelete;
    public const string CategoriesRead = PermissionNames.CategoriesRead;
    public const string CategoriesCreate = PermissionNames.CategoriesCreate;
    public const string CategoriesUpdate = PermissionNames.CategoriesUpdate;
    public const string CategoriesDelete = PermissionNames.CategoriesDelete;
    public const string OrdersRead = PermissionNames.OrdersRead;
    public const string OrdersCreate = PermissionNames.OrdersCreate;
    public const string OrdersUpdate = PermissionNames.OrdersUpdate;
    public const string PaymentsCreate = PermissionNames.PaymentsCreate;
    public const string BasketWrite = PermissionNames.BasketWrite;
    public const string DeliveryMethodsRead = PermissionNames.DeliveryMethodsRead;
    public const string DeliveryMethodsCreate = PermissionNames.DeliveryMethodsCreate;
    public const string DeliveryMethodsUpdate = PermissionNames.DeliveryMethodsUpdate;
    public const string DeliveryMethodsDelete = PermissionNames.DeliveryMethodsDelete;
}
