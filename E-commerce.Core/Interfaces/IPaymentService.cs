namespace E_commerce.Core.Interfaces;

public interface IPaymentService
{
    Task<CustomerBasket> CreateOrUpdatePaymentAsync(string basketId, int? deliveryMethod);
}
