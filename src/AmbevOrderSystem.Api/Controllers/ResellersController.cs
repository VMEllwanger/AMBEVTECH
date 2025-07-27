using AmbevOrderSystem.Api.Controllers.Base;
using AmbevOrderSystem.Api.Mappers;
using AmbevOrderSystem.Api.Resquet;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Models.Responses.Reseller;
using Microsoft.AspNetCore.Mvc;

namespace AmbevOrderSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResellersController : HelperController
    {
        private readonly IUseCase<CreateResellerCommand, ResellerResponse> _createResellerUseCase;
        private readonly IUseCase<GetResellerByIdCommand, ResellerResponse> _getResellerByIdUseCase;
        private readonly IUseCase<GetResellerByCnpjCommand, ResellerResponse> _getResellerByCnpjUseCase;
        private readonly IUseCase<GetAllResellersCommand, GetAllResellersResponse> _getAllResellersUseCase;
        private readonly IUseCase<UpdateResellerCommand, ResellerResponse> _updateResellerUseCase;
        private readonly IUseCase<DeleteResellerCommand, DeleteResellerResponse> _deleteResellerUseCase;
        private readonly ILogger<ResellersController> _logger;

        public ResellersController(
            IUseCase<CreateResellerCommand, ResellerResponse> createResellerUseCase,
            IUseCase<GetResellerByIdCommand, ResellerResponse> getResellerByIdUseCase,
            IUseCase<GetResellerByCnpjCommand, ResellerResponse> getResellerByCnpjUseCase,
            IUseCase<GetAllResellersCommand, GetAllResellersResponse> getAllResellersUseCase,
            IUseCase<UpdateResellerCommand, ResellerResponse> updateResellerUseCase,
            IUseCase<DeleteResellerCommand, DeleteResellerResponse> deleteResellerUseCase,
            ILogger<ResellersController> logger)
        {
            _createResellerUseCase = createResellerUseCase;
            _getResellerByIdUseCase = getResellerByIdUseCase;
            _getResellerByCnpjUseCase = getResellerByCnpjUseCase;
            _getAllResellersUseCase = getAllResellersUseCase;
            _updateResellerUseCase = updateResellerUseCase;
            _deleteResellerUseCase = deleteResellerUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Criar nova revenda
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateReseller([FromBody] CreateResellerRequest request)
        {
            _logger.LogInformation("Recebida requisição para criar reseller: {Cnpj}", request.Cnpj);

            var command = RequestMapper.ToCommand(request);
            var result = await _createResellerUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                if (result.ValidationErrors?.Any() == true)
                {
                    _logger.LogWarning("Erro de validação ao criar reseller: {Errors}",
                        string.Join(", ", result.ValidationErrors));
                }
                else
                {
                    _logger.LogError("Erro ao criar reseller: {Error}", result.ErrorMessage);
                }

                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Reseller criado com sucesso: {Id}", result.Data!.Id);
            return CreatedAtAction(nameof(GetReseller), new { id = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Obter revenda por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReseller(int id)
        {
            _logger.LogInformation("Recebida requisição para obter reseller por ID: {Id}", id);

            var command = new GetResellerByIdCommand { Id = id };
            var result = await _getResellerByIdUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Reseller com ID {Id} não encontrado", id);
                return GenerateErrorResponse(result);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Obter revenda por CNPJ
        /// </summary>
        [HttpGet("cnpj/{cnpj}")]
        public async Task<IActionResult> GetResellerByCnpj(string cnpj)
        {
            _logger.LogInformation("Recebida requisição para obter reseller por CNPJ: {Cnpj}", cnpj);

            var command = new GetResellerByCnpjCommand { Cnpj = cnpj };
            var result = await _getResellerByCnpjUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Reseller com CNPJ {Cnpj} não encontrado", cnpj);
                return GenerateErrorResponse(result);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Listar todas as revendas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllResellers()
        {
            _logger.LogInformation("Recebida requisição para obter todos os resellers");

            var command = new GetAllResellersCommand();
            var result = await _getAllResellersUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogError("Erro ao obter todos os resellers: {Error}", result.ErrorMessage);
                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Retornados {Count} resellers", result.Data!.TotalCount);
            return Ok(result.Data);
        }

        /// <summary>
        /// Atualizar revenda
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReseller(int id, [FromBody] UpdateResellerRequest request)
        {
            _logger.LogInformation("Recebida requisição para atualizar reseller ID: {Id}", id);

            var command = RequestMapper.ToCommand(request, id);
            var result = await _updateResellerUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                if (result.ValidationErrors?.Any() == true)
                {
                    _logger.LogWarning("Erro de validação ao atualizar reseller {Id}: {Errors}",
                        id, string.Join(", ", result.ValidationErrors));
                }
                else
                {
                    _logger.LogError("Erro ao atualizar reseller {Id}: {Error}", id, result.ErrorMessage);
                }

                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Reseller {Id} atualizado com sucesso", id);
            return Ok(result.Data);
        }

        /// <summary>
        /// Deletar revenda
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReseller(int id)
        {
            _logger.LogInformation("Recebida requisição para deletar reseller ID: {Id}", id);

            var command = new DeleteResellerCommand { Id = id };
            var result = await _deleteResellerUseCase.ExecuteAsync(command);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Erro ao deletar reseller {Id}: {Error}", id, result.ErrorMessage);
                return GenerateErrorResponse(result);
            }

            _logger.LogInformation("Reseller {Id} deletado com sucesso", id);
            return NoContent();
        }
    }
}