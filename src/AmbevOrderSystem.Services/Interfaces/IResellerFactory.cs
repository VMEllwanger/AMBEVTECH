using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;

namespace AmbevOrderSystem.Services.Interfaces
{
    public interface IResellerFactory
    {
        Task<Reseller> CreateAsync(CreateResellerRequest request);
        Task<Reseller> UpdateAsync(Reseller existingReseller, UpdateResellerRequest request);
    }
}