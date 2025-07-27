using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Mappers;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Models.Responses.Reseller;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Reseller
{
    /// <summary>
    /// UseCase para obter revenda por ID
    /// </summary>
    public class GetResellerByIdUseCase : IUseCase<GetResellerByIdCommand, ResellerResponse>
    {
        private readonly IResellerRepository _resellerRepository;
        private readonly ILogger<GetResellerByIdUseCase> _logger;

        public GetResellerByIdUseCase(
            IResellerRepository resellerRepository,
            ILogger<GetResellerByIdUseCase> logger)
        {
            _resellerRepository = resellerRepository;
            _logger = logger;
        }

        public async Task<Result<ResellerResponse>> ExecuteAsync(GetResellerByIdCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase GetResellerById para ID {Id}", command.Id);

                var reseller = await _resellerRepository.GetByIdAsync(command.Id);
                if (reseller == null)
                {
                    _logger.LogWarning("Reseller com ID {Id} não encontrado", command.Id);
                    return Result<ResellerResponse>.Failure("Revenda não encontrada");
                }

                var response = ResellerMapper.ToCreateResponse(reseller);

                _logger.LogInformation("Reseller {Id} encontrado com sucesso", command.Id);
                return Result<ResellerResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase GetResellerById para ID {Id}", command.Id);
                return Result<ResellerResponse>.Failure("Erro interno do servidor");
            }
        }
    }
}