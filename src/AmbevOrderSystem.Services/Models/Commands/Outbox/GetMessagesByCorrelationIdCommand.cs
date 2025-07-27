namespace AmbevOrderSystem.Services.Models.Commands.Outbox
{
    /// <summary>
    /// Comando para buscar mensagens por CorrelationId
    /// </summary>
    public class GetMessagesByCorrelationIdCommand : BaseCommand
    {
        public string CorrelationId { get; set; } = string.Empty;

        public GetMessagesByCorrelationIdCommand(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}