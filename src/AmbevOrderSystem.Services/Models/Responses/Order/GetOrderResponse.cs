namespace AmbevOrderSystem.Services.Models.Responses.Order
{
    /// <summary>
    /// Response para buscar pedido
    /// </summary>
    public class GetOrderResponse : BaseResponse
    {
        public int Id { get; set; }
        public int ResellerId { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public List<OrderItemResponse> Items { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public string? AmbevOrderNumber { get; set; }
        public DateTime OrderCreatedAt { get; set; }
    }
}