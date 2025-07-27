using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;

namespace AmbevOrderSystem.Services.Interfaces
{
    public interface IOrderFactory
    {
        Task<CustomerOrder> CreateAsync(int resellerId, CreateCustomerOrderRequest request);
    }
}