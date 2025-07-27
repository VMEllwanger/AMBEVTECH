using AmbevOrderSystem.Services.Models.Responses;

namespace AmbevOrderSystem.Services.Models.Responses
{
    /// <summary>
    /// Response para criação de pedido de cliente
    /// </summary>
    public class CreateCustomerOrderResponse : BaseResponse
    {
        public int OrderId { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public List<OrderItemResponse> Items { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime OrderCreatedAt { get; set; }
    }

    /// <summary>
    /// Response para item do pedido
    /// </summary>
    public class OrderItemResponse
    {
        public int Id { get; set; }
        public string ProductSku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}