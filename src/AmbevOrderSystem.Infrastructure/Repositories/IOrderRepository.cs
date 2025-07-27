using AmbevOrderSystem.Infrastructure.Entities;

namespace AmbevOrderSystem.Infrastructure.Repositories
{
    public interface IOrderRepository : IRepository<CustomerOrder>
    {
        Task<IEnumerable<CustomerOrder>> GetByResellerIdAsync(int resellerId);
        Task<IEnumerable<CustomerOrder>> GetPendingOrdersAsync();
        Task<IEnumerable<CustomerOrder>> GetOrdersByStatusAsync(OrderStatus status);
    }
}