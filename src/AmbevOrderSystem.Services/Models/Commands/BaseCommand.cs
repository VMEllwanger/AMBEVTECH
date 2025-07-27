namespace AmbevOrderSystem.Services.Models.Commands
{
    /// <summary>
    /// Classe base para comandos no padrão UseCase
    /// </summary>
    public abstract class BaseCommand
    {
        /// <summary>
        /// Identificador único do comando
        /// </summary>
        public Guid CommandId { get; } = Guid.NewGuid();

        /// <summary>
        /// Data e hora de criação do comando
        /// </summary>
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
    }
}