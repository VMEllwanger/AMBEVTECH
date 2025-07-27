﻿namespace AmbevOrderSystem.Api.Resquet
{
    public class OrderItemRequest
    {
        public int Id { get; set; }
        public string ProductSku { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
