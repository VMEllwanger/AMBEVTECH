namespace AmbevOrderSystem.Services.Models.Commands.Reseller
{
    /// <summary>
    /// Comando para deletar uma revenda
    /// </summary>
    public class DeleteResellerCommand : BaseCommand
    {
        public int Id { get; set; }
    }
}