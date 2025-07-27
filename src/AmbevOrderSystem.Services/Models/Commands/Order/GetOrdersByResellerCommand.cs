namespace AmbevOrderSystem.Services.Models.Commands.Order
{
    /// <summary>
    /// Comando para buscar pedidos por revenda
    /// </summary>
    public class GetOrdersByResellerCommand : BaseCommand
    {
        public int ResellerId { get; set; }
    }
}