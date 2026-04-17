using E_commerce.Core.Entities.Order;

namespace E_commerce.Application.Contracts.Order;

public record UpdateOrderStatusRequest(Status Status);
