namespace AmbevOrderSystem.Services.Models.Responses.Order
{
    /// <summary>
    /// Response para buscar todos os pedidos
    /// </summary>
    public class GetAllOrdersResponse : BaseResponse
    {
        public List<GetOrderResponse> Orders { get; set; } = new();
    }
}