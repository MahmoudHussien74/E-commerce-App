using E_commerce.Core.Contracts.Order;
using E_commerce.Core.Entities.Order;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.Infrastructure.Repositories;

public class OrderRepository(ApplicationDbContext context) : GenericRepository<Orders>(context), IOrderRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IReadOnlyList<OrderResponse>> GetAllOrdersForUserProjectedAsync(string buyerEmail)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.BuyerEmail == buyerEmail)
            .OrderByDescending(o => o.OrderDate)
            .Select(o => new OrderResponse
            {
                Id = o.Id,
                BuyerEmail = o.BuyerEmail,
                OrderDate = o.OrderDate,
                ShippingAddress = new AddressDto
                {
                    FirstName = o.ShippingAddress.FirstName,
                    LastName = o.ShippingAddress.LastName,
                    Street = o.ShippingAddress.Street,
                    City = o.ShippingAddress.City,
                    State = o.ShippingAddress.State,
                    ZipCode = o.ShippingAddress.ZipCode
                },
                DeliveryMethod = o.DeliveryMethod!.ShortName ?? string.Empty,
                ShippingPrice = o.DeliveryMethod!.Price,
                SubTotal = o.SubTotal,
                Total = o.SubTotal + o.DeliveryMethod!.Price,
                Status = o.Status.ToString(),
                OrderItems = o.OrderItems.Select(oi => new OrderItemResponse
                {
                    ProductItemId = oi.ProductItemId,
                    ProductName = oi.ProductName,
                    MainImage = oi.MainImage,
                    Price = oi.Price,
                    Quntity = oi.Quntity
                }).ToList()
            })
            .ToListAsync();
    }
    public async Task<OrderResponse> GetOrdersForUserProjectedAsync(string buyerEmail, int id)
        => await _context.Orders
            .AsNoTracking()
            .Where(x => x.BuyerEmail == buyerEmail && x.Id == id)
            .Select(x => new OrderResponse
            {
                Id = x.Id,
                BuyerEmail = x.BuyerEmail,
                OrderDate = x.OrderDate,
                ShippingAddress = new AddressDto
                {
                    FirstName = x.ShippingAddress.FirstName,
                    LastName = x.ShippingAddress.LastName,
                    City = x.ShippingAddress.City,
                    State = x.ShippingAddress.State,
                    Street = x.ShippingAddress.Street,
                    ZipCode = x.ShippingAddress.ZipCode
                },
                DeliveryMethod = x.DeliveryMethod!.ShortName ?? string.Empty,
                ShippingPrice = x.DeliveryMethod!.Price,
                SubTotal = x.SubTotal,
                Total = x.SubTotal + x.DeliveryMethod!.Price,
                Status = x.Status.ToString(),
                OrderItems = x.OrderItems.Select(oi => new OrderItemResponse
                {
                    ProductItemId = oi.ProductItemId,
                    ProductName = oi.ProductName,
                    MainImage = oi.MainImage,
                    Price = oi.Price,
                    Quntity = oi.Quntity
                }).ToList(),
            }).FirstOrDefaultAsync();

    public async Task<IReadOnlyList<DeliveryMethodResponse>> GetDeliveryMethodsProjectedAsync()
        => await _context.DeliveryMethods
                         .AsNoTracking()
                         .Select(x => new DeliveryMethodResponse
                         {
                             Id = x.Id,
                             ShortName = x.ShortName,
                             Description = x.Description,
                             DeliveryTime = x.DeliveryTime,
                             Price = x.Price
                         }).ToListAsync();
}