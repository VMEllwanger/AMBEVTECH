namespace AmbevOrderSystem.Services.Models.Responses.Order
{
    /// <summary>
    /// Response para processar pedidos pendentes
    /// </summary>
    public class ProcessPendingOrdersResponse : BaseResponse
    {
        public int ProcessedOrders { get; set; }
        public int SentToAmbev { get; set; }
        public int FailedOrders { get; set; }
    }
}