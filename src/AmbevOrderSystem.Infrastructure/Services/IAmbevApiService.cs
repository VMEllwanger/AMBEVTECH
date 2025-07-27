using AmbevOrderSystem.Infrastructure.DTOs;

namespace AmbevOrderSystem.Infrastructure.Services
{
    public interface IAmbevApiService
    {
        Task<AmbevOrderResponse> SubmitOrderAsync(AmbevOrderRequest request);
    }
}