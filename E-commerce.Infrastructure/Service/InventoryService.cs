using E_commerce.Core.Entities.Order;
using Microsoft.Extensions.Logging;

namespace E_commerce.Infrastructure.Service;

public class InventoryService(IUnitOfWork unitOfWork, ILogger<InventoryService> logger) : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<InventoryService> _logger = logger;

    public async Task<bool> DeductStockAsync(IEnumerable<OrderItem> items, CancellationToken cancellationToken = default)
    {
        try
        {
            foreach (var item in items)
            {
                var product = await _unitOfWork.ProductRepository.GetByIdAsync(item.ProductItemId, cancellationToken);
                
                if (product == null)
                {
                    _logger.LogError("Product {ProductId} not found during stock deduction!", item.ProductItemId);
                    return false; 
                }

                if (product.StockQuantity < item.Quntity)
                {
                    _logger.LogCritical("Insufficient stock for product {ProductName}. Available: {Available}, Requested: {Requested}", 
                        product.Name, product.StockQuantity, item.Quntity);
                    return false;
                }

                _logger.LogInformation("Deducting stock for {ProductName}: {Old} -> {New}", 
                    product.Name, product.StockQuantity, product.StockQuantity - item.Quntity);
                
                product.StockQuantity -= item.Quntity;
                _unitOfWork.ProductRepository.Update(product);
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Transactional failure during stock deduction.");
            return false;
        }
    }
}
