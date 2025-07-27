using AmbevOrderSystem.Services.Models.Result;

namespace AmbevOrderSystem.Services.Interfaces
{
    /// <summary>
    /// Interface genérica para implementação do padrão UseCase
    /// </summary>
    /// <typeparam name="TCommand">Tipo do comando de entrada</typeparam>
    /// <typeparam name="TResponse">Tipo da resposta</typeparam>
    public interface IUseCase<TCommand, TResponse>
    {
        /// <summary>
        /// Executa o caso de uso
        /// </summary>
        /// <param name="command">Comando com os dados de entrada</param>
        /// <returns>Resultado da operação</returns>
        Task<Result<TResponse>> ExecuteAsync(TCommand command);
    }
}