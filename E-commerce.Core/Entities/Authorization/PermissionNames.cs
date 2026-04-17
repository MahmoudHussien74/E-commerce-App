namespace E_commerce.Core.Entities.Authorization;

public static class PermissionNames
{
    public const string ClaimType = "permission";

    public const string ProductsRead = "products:read";
    public const string ProductsCreate = "products:create";
    public const string ProductsUpdate = "products:update";
    public const string ProductsDelete = "products:delete";

    public const string CategoriesRead = "categories:read";
    public const string CategoriesCreate = "categories:create";
    public const string CategoriesUpdate = "categories:update";
    public const string CategoriesDelete = "categories:delete";

    public const string OrdersRead = "orders:read";
    public const string OrdersCreate = "orders:create";
    public const string PaymentsCreate = "payments:create";
    public const string BasketWrite = "basket:write";
    public const string OrdersUpdate = "orders:update";

    public const string DeliveryMethodsRead = "deliverymethods:read";
    public const string DeliveryMethodsCreate = "deliverymethods:create";
    public const string DeliveryMethodsUpdate = "deliverymethods:update";
    public const string DeliveryMethodsDelete = "deliverymethods:delete";

    public static readonly IReadOnlyList<(string Name, string Description)> All =
    [
        (ProductsRead, "Read products"),
        (ProductsCreate, "Create products"),
        (ProductsUpdate, "Update products"),
        (ProductsDelete, "Delete products"),
        (CategoriesRead, "Read categories"),
        (CategoriesCreate, "Create categories"),
        (CategoriesUpdate, "Update categories"),
        (CategoriesDelete, "Delete categories"),
        (OrdersRead, "Read orders"),
        (OrdersCreate, "Create orders"),
        (OrdersUpdate, "Update orders"),
        (PaymentsCreate, "Create and update payments"),
        (BasketWrite, "Manage customer baskets"),
        (DeliveryMethodsRead, "Read delivery methods"),
        (DeliveryMethodsCreate, "Create delivery methods"),
        (DeliveryMethodsUpdate, "Update delivery methods"),
        (DeliveryMethodsDelete, "Delete delivery methods")
    ];
}
