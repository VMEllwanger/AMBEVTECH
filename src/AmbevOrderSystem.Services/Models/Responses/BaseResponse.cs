namespace AmbevOrderSystem.Services.Models.Responses
{
    /// <summary>
    /// Classe base para respostas no padrão UseCase
    /// </summary>
    public abstract class BaseResponse
    {
        /// <summary>
        /// Identificador único da resposta
        /// </summary>
        public Guid ResponseId { get; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora de criação da resposta
        /// </summary>
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}