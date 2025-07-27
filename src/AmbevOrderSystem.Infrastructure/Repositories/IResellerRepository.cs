using AmbevOrderSystem.Infrastructure.Entities;

namespace AmbevOrderSystem.Infrastructure.Repositories
{
    public interface IResellerRepository : IRepository<Reseller>
    {
        Task<Reseller?> GetByCnpjAsync(string cnpj);
        Task<bool> ExistsByCnpjAsync(string cnpj);
    }
}