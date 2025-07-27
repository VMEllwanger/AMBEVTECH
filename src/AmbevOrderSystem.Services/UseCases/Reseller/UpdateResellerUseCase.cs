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
    /// UseCase para atualizar uma revenda
    /// </summary>
    public class UpdateResellerUseCase : IUseCase<UpdateResellerCommand, ResellerResponse>
    {
        private readonly IResellerRepository _resellerRepository;
        private readonly ILogger<UpdateResellerUseCase> _logger;

        public UpdateResellerUseCase(
            IResellerRepository resellerRepository,
            ILogger<UpdateResellerUseCase> logger)
        {
            _resellerRepository = resellerRepository;
            _logger = logger;
        }

        public async Task<Result<ResellerResponse>> ExecuteAsync(UpdateResellerCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase UpdateReseller para ID {Id}", command.Id);

                var existingReseller = await _resellerRepository.GetByIdAsync(command.Id);
                if (existingReseller == null)
                {
                    _logger.LogWarning("Reseller com ID {Id} não encontrado para atualização", command.Id);
                    return Result<ResellerResponse>.Failure("Revenda não encontrada");
                }

                if (command.Cnpj != existingReseller.Cnpj)
                {
                    var resellerWithSameCnpj = await _resellerRepository.GetByCnpjAsync(command.Cnpj);
                    if (resellerWithSameCnpj != null && resellerWithSameCnpj.Id != command.Id)
                    {
                        _logger.LogWarning("Tentativa de atualizar reseller com CNPJ já existente: {Cnpj}", command.Cnpj);
                        return Result<ResellerResponse>.Failure("CNPJ já cadastrado em outro sistema");
                    }
                }

                ResellerMapper.UpdateEntity(existingReseller, command);

                var updatedReseller = await _resellerRepository.UpdateAsync(existingReseller);

                var response = ResellerMapper.ToCreateResponse(updatedReseller);

                _logger.LogInformation("Reseller {Id} atualizado com sucesso", command.Id);
                return Result<ResellerResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase UpdateReseller para ID {Id}", command.Id);
                return Result<ResellerResponse>.Failure("Erro interno do servidor");
            }
        }
    }
}