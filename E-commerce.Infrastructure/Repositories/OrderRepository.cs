using E_commerce.Core.Entities.Order;

namespace E_commerce.Infrastructure.Repositories;

internal sealed class OrderRepository(ApplicationDbContext context) : GenericRepository<Orders>(context), IOrderRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IReadOnlyList<Orders>> GetAllOrdersForUserAsync(string buyerId, CancellationToken cancellationToken = default)
        => await _context.Orders
            .AsNoTracking()
            .Where(o => o.BuyerId == buyerId)
            .Include(o => o.OrderItems)
            .Include(o => o.DeliveryMethod)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

    public async Task<Orders?> GetOrderForUserAsync(string buyerId, int id, CancellationToken cancellationToken = default)
        => await _context.Orders
            .AsNoTracking()
            .Where(x => x.BuyerId == buyerId && x.Id == id)
            .Include(x => x.OrderItems)
            .Include(x => x.DeliveryMethod)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Orders?> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
        => await _context.Orders
            .Include(x => x.OrderItems)
            .Include(x => x.DeliveryMethod)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}
