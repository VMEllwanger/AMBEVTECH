namespace AmbevOrderSystem.Api.Resquet
{
    public class CreateCustomerOrderRequest
    {
        public string CustomerIdentification { get; set; } = string.Empty;
        public List<OrderItemRequest> Items { get; set; } = new();
    }
}