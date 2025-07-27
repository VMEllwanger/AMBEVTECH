using AmbevOrderSystem.Infrastructure.DTOs;
using Microsoft.Extensions.Logging;
using Polly.Registry;

namespace AmbevOrderSystem.Infrastructure.Services
{
    public class MockAmbevApiService : IAmbevApiService
    {
        private readonly ILogger<MockAmbevApiService> _logger;
        private readonly ResiliencePipelineProvider<string> _resiliencePolicies;

        public MockAmbevApiService(ILogger<MockAmbevApiService> logger, ResiliencePipelineProvider<string> resiliencePipeline)
        {
            _logger = logger;
            _resiliencePolicies = resiliencePipeline;
        }

        public async Task<AmbevOrderResponse> SubmitOrderAsync(AmbevOrderRequest request)
        {
            _logger.LogInformation("Mock: Enviando pedido para API Ambev. CNPJ: {Cnpj}", request.ResellerCnpj);

            return await _resiliencePolicies.GetPipeline<AmbevOrderResponse>("ambev-api-policy").ExecuteAsync(async (ct) =>
            {
                await SimulateMockApiInstability();

                await Task.Delay(500);

                var response = new AmbevOrderResponse
                {
                    OrderNumber = $"AMB-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}",
                    Items = request.Items,
                    CreatedAt = DateTime.UtcNow
                };

                _logger.LogInformation("Mock: Pedido enviado com sucesso. CNPJ: {Cnpj}, OrderNumber: {OrderNumber}",
                    request.ResellerCnpj, response.OrderNumber);

                return response;
            });
        }

        private async Task SimulateMockApiInstability()
        {
            var random = new Random();

            if (random.Next(1, 100) <= 30)
            {
                _logger.LogWarning("Mock: Simulando falha temporária - Serviço indisponível");
                throw new HttpRequestException("Mock: API Ambev temporariamente indisponível");
            }

            await Task.Delay(random.Next(100, 800));
        }
    }
}