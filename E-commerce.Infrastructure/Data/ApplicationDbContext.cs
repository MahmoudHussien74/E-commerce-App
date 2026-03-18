using E_commerce.Core.Entities.Product;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
namespace E_commerce.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :DbContext(options)
{
   public DbSet<Product> Products {  get; set; }
   public DbSet<Category>  Categories {  get; set; }
   public DbSet<Photo>  Photos {  get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
   
}
