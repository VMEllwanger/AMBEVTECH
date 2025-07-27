using AmbevOrderSystem.Infrastructure.Data;
using AmbevOrderSystem.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace AmbevOrderSystem.Infrastructure.Repositories
{
    public class ResellerRepository : BaseRepository<Reseller>, IResellerRepository
    {
        public ResellerRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Reseller?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Phones)
                .Include(r => r.Contacts)
                .Include(r => r.DeliveryAddresses)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public override async Task<IEnumerable<Reseller>> GetAllAsync()
        {
            return await _dbSet
                .Include(r => r.Phones)
                .Include(r => r.Contacts)
                .Include(r => r.DeliveryAddresses)
                .ToListAsync();
        }

        public async Task<Reseller?> GetByCnpjAsync(string cnpj)
        {
            var cleanCnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            return await _dbSet
                .Include(r => r.Phones)
                .Include(r => r.Contacts)
                .Include(r => r.DeliveryAddresses)
                .FirstOrDefaultAsync(r => r.Cnpj == cleanCnpj);
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj)
        {
            var cleanCnpj = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            return await _dbSet.AnyAsync(r => r.Cnpj == cleanCnpj);
        }
    }
}