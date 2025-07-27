using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Models.Responses.Reseller;
using AmbevOrderSystem.Services.Models.Result;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.UseCases.Reseller
{
    /// <summary>
    /// UseCase para deletar uma revenda
    /// </summary>
    public class DeleteResellerUseCase : IUseCase<DeleteResellerCommand, DeleteResellerResponse>
    {
        private readonly IResellerRepository _resellerRepository;
        private readonly ILogger<DeleteResellerUseCase> _logger;

        public DeleteResellerUseCase(
            IResellerRepository resellerRepository,
            ILogger<DeleteResellerUseCase> logger)
        {
            _resellerRepository = resellerRepository;
            _logger = logger;
        }

        public async Task<Result<DeleteResellerResponse>> ExecuteAsync(DeleteResellerCommand command)
        {
            try
            {
                _logger.LogInformation("Executando UseCase DeleteReseller para ID {Id}", command.Id);

                var existingReseller = await _resellerRepository.GetByIdAsync(command.Id);
                if (existingReseller == null)
                {
                    _logger.LogWarning("Reseller com ID {Id} não encontrado para exclusão", command.Id);
                    return Result<DeleteResellerResponse>.Failure("Revenda não encontrada");
                }

                await _resellerRepository.DeleteAsync(existingReseller);

                var response = new DeleteResellerResponse
                {
                    Id = command.Id,
                    Message = "Revenda deletada com sucesso",
                    IsDeleted = true
                };

                _logger.LogInformation("Reseller {Id} deletado com sucesso", command.Id);
                return Result<DeleteResellerResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar UseCase DeleteReseller para ID {Id}", command.Id);
                return Result<DeleteResellerResponse>.Failure("Erro interno do servidor");
            }
        }
    }
}