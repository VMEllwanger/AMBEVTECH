using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Interfaces;

namespace AmbevOrderSystem.Services.Factories
{
    public class OrderFactory : IOrderFactory
    {
        public Task<CustomerOrder> CreateAsync(int resellerId, CreateCustomerOrderRequest request)
        {
            var order = new CustomerOrder
            {
                CustomerIdentification = request.CustomerIdentification,
                ResellerId = resellerId,
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductSku = i.ProductSku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return Task.FromResult(order);
        }
    }
}