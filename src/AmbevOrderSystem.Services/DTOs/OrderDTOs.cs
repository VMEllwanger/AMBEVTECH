namespace AmbevOrderSystem.Services.DTOs
{
    public class CreateCustomerOrderRequest
    {
        public string CustomerIdentification { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class CustomerOrderResponse
    {
        public int OrderId { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CustomerOrderDto
    {
        public int Id { get; set; }
        public string CustomerIdentification { get; set; } = string.Empty;
        public int ResellerId { get; set; }
        public string ResellerName { get; set; } = string.Empty;
        public List<OrderItemDto> Items { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? AmbevOrderNumber { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public string ProductSku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}