namespace AmbevOrderSystem.Services.Models.Commands.Order
{
    /// <summary>
    /// Comando para buscar pedido por ID
    /// </summary>
    public class GetOrderByIdCommand : BaseCommand
    {
        public int OrderId { get; set; }
    }
}