using System.Text.Json.Serialization;

namespace AmbevOrderSystem.Infrastructure.DTOs
{
    /// <summary>
    /// DTO para item de pedido
    /// </summary>
    public class OrderItemDto
    {
        [JsonPropertyName("productSku")]
        public string ProductSku { get; set; } = string.Empty;

        [JsonPropertyName("productName")]
        public string ProductName { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }

        [JsonPropertyName("unitPrice")]
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// Request para envio de pedido para API Ambev
    /// </summary>
    public class AmbevOrderRequest
    {
        [JsonPropertyName("resellerCnpj")]
        public string ResellerCnpj { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<OrderItemDto> Items { get; set; } = new();
    }

    /// <summary>
    /// Response da API Ambev
    /// </summary>
    public class AmbevOrderResponse
    {
        [JsonPropertyName("orderNumber")]
        public string OrderNumber { get; set; } = string.Empty;

        [JsonPropertyName("items")]
        public List<OrderItemDto> Items { get; set; } = new();

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}