using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Result;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.Decorators
{
    /// <summary>
    /// Decorator para validação de comandos antes da execução do UseCase
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando</typeparam>
    /// <typeparam name="TResponse">Tipo da resposta</typeparam>
    public sealed class ValidationDecorator<TCommand, TResponse> : IUseCase<TCommand, TResponse>
        where TCommand : notnull
    {
        private readonly AbstractValidator<TCommand> _validator;
        private readonly IUseCase<TCommand, TResponse> _useCase;
        private readonly ILogger<ValidationDecorator<TCommand, TResponse>> _logger;

        public ValidationDecorator(
            AbstractValidator<TCommand> validator,
            IUseCase<TCommand, TResponse> useCase,
            ILogger<ValidationDecorator<TCommand, TResponse>> logger)
        {
            _validator = validator;
            _useCase = useCase;
            _logger = logger;
        }

        public async Task<Result<TResponse>> ExecuteAsync(TCommand command)
        {
            _logger.LogInformation("Iniciando validação do comando {CommandType}", typeof(TCommand).Name);

            var validationResult = await _validator.ValidateAsync(command);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validação do comando {CommandType} falhou. Erros: {Errors}",
                    typeof(TCommand).Name,
                    string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));

                var errorMessages = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return Result<TResponse>.ValidationFailure(errorMessages);
            }

            _logger.LogInformation("Validação do comando {CommandType} executada com sucesso", typeof(TCommand).Name);

            return await _useCase.ExecuteAsync(command);
        }
    }
}