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
    /// UseCase para obter revenda por CNPJ
    /// </summary>
    public class GetResellerByCnpjUseCase : IUseCase<GetResellerByCnpjCommand, ResellerResponse>
    {
        private readonly IResellerRepository _resellerRepository;
        private readonly ILogger<GetResellerByCnpjUseCase> _logger;

        public GetResellerByCnpjUseCase(
            IResellerRepository resellerRepository,
            ILogger<GetResellerByCnpjUseCase> logger)
        {
            _resellerRepository = resellerRepository;
            _logger = logger;
        }

        public async Task<Result<ResellerResponse>> ExecuteAsync(GetResellerByCnpjCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase GetResellerByCnpj para CNPJ {Cnpj}", command.Cnpj);

                var reseller = await _resellerRepository.GetByCnpjAsync(command.Cnpj);
                if (reseller == null)
                {
                    _logger.LogWarning("Reseller com CNPJ {Cnpj} não encontrado", command.Cnpj);
                    return Result<ResellerResponse>.Failure("Revenda não encontrada");
                }

                var response = ResellerMapper.ToCreateResponse(reseller);

                _logger.LogInformation("Reseller com CNPJ {Cnpj} encontrado com sucesso", command.Cnpj);
                return Result<ResellerResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase GetResellerByCnpj para CNPJ {Cnpj}", command.Cnpj);
                return Result<ResellerResponse>.Failure("Erro interno do servidor");
            }
        }
    }
}