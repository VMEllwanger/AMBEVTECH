using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Mappers;
using AmbevOrderSystem.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using AmbevOrderSystem.Services.Models.Result;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Models.Responses.Reseller;

namespace AmbevOrderSystem.Services.UseCases.Reseller
{
    /// <summary>
    /// UseCase para obter todas as revendas
    /// </summary>
    public class GetAllResellersUseCase : IUseCase<GetAllResellersCommand, GetAllResellersResponse>
    {
        private readonly IResellerRepository _resellerRepository;
        private readonly ILogger<GetAllResellersUseCase> _logger;

        public GetAllResellersUseCase(
            IResellerRepository resellerRepository,
            ILogger<GetAllResellersUseCase> logger)
        {
            _resellerRepository = resellerRepository;
            _logger = logger;
        }

        public async Task<Result<GetAllResellersResponse>> ExecuteAsync(GetAllResellersCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase GetAllResellers para comando {CommandId}", command.CommandId);

                var resellers = await _resellerRepository.GetAllAsync();
                var resellerResponses = ResellerMapper.ToGetResponseList(resellers);

                var response = new GetAllResellersResponse
                {
                    Resellers = resellerResponses,
                    TotalCount = resellerResponses.Count
                };

                _logger.LogInformation("Encontrados {Count} resellers", response.TotalCount);
                return Result<GetAllResellersResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase GetAllResellers para comando {CommandId}", command.CommandId);
                return Result<GetAllResellersResponse>.Failure("Erro interno do servidor");
            }
        }
    }
}