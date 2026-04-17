namespace E_commerce.Core.Entities
{
    public static class Permissions
    {
        public static string Type { get; } = "permission";

        public const string ProductsRead = "products:read";
        public const string ProductsCreate = "products:create";
        public const string ProductsUpdate = "products:update";
        public const string ProductsDelete = "products:delete";

        public const string OrdersRead = "orders:read";
        public const string OrdersCreate = "orders:create";
        public const string OrdersUpdate = "orders:update";
        public const string OrdersDelete = "orders:delete";

        public static IList<string> GetAllPermissions() => typeof(Permissions)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Where(f => f.FieldType == typeof(string) && f.Name != nameof(Type))
            .Select(f => f.GetValue(null) as string)
            .Where(s => s != null)
            .ToList()!;
    }
}