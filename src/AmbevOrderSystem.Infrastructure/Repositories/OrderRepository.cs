using AmbevOrderSystem.Infrastructure.Data;
using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmbevOrderSystem.Infrastructure.Repositories
{
    public class OrderRepository : BaseRepository<CustomerOrder>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<CustomerOrder?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(o => o.Items)
                .Include(o => o.Reseller)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<IEnumerable<CustomerOrder>> GetByResellerIdAsync(int resellerId)
        {
            return await _dbSet
                .Include(o => o.Items)
                .Where(o => o.ResellerId == resellerId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrder>> GetPendingOrdersAsync()
        {
            return await _dbSet
                .Include(o => o.Items)
                .Include(o => o.Reseller)
                .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Retry)
                .ToListAsync();
        }

        public async Task<IEnumerable<CustomerOrder>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Include(o => o.Items)
                .Include(o => o.Reseller)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}