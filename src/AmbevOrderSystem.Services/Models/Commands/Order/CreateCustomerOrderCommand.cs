namespace AmbevOrderSystem.Services.Models.Commands.Order
{
    /// <summary>
    /// Comando para criação de pedido de cliente
    /// </summary>
    public class CreateCustomerOrderCommand : BaseCommand
    {
        public int ResellerId { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public List<OrderItemCommand> Items { get; set; } = new();
    }

    /// <summary>
    /// Comando para item do pedido
    /// </summary>
    public class OrderItemCommand
    {
        public string ProductSku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}