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
    /// UseCase para criação de uma nova revenda
    /// </summary>
    public class CreateResellerUseCase : IUseCase<CreateResellerCommand, ResellerResponse>
    {
        private readonly IResellerRepository _resellerRepository;
        private readonly ILogger<CreateResellerUseCase> _logger;

        public CreateResellerUseCase(
            IResellerRepository resellerRepository,
            ILogger<CreateResellerUseCase> logger)
        {
            _resellerRepository = resellerRepository;
            _logger = logger;
        }

        public async Task<Result<ResellerResponse>> ExecuteAsync(CreateResellerCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase CreateReseller para comando {CommandId}", command.CommandId);

                var existingReseller = await _resellerRepository.GetByCnpjAsync(command.Cnpj);
                if (existingReseller != null)
                {
                    _logger.LogWarning("Tentativa de criar reseller com CNPJ já existente: {Cnpj}", command.Cnpj);
                    return Result<ResellerResponse>.Failure("CNPJ já cadastrado no sistema");
                }

                var resellerEntity = ResellerMapper.ToEntity(command);

                var createdReseller = await _resellerRepository.AddAsync(resellerEntity);

                var response = ResellerMapper.ToCreateResponse(createdReseller);

                _logger.LogInformation("Reseller criado com sucesso. ID: {Id}, CNPJ: {Cnpj}",
                    createdReseller.Id, createdReseller.Cnpj);

                return Result<ResellerResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase CreateReseller para comando {CommandId}", command.CommandId);
                return Result<ResellerResponse>.Failure("Erro interno do servidor");
            }
        }
    }
}