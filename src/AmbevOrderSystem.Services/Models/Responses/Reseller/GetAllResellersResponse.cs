namespace AmbevOrderSystem.Services.Models.Responses.Reseller
{
    /// <summary>
    /// Resposta para obter todas as revendas
    /// </summary>
    public class GetAllResellersResponse : BaseResponse
    {
        public List<ResellerResponse> Resellers { get; set; } = new();
        public int TotalCount { get; set; }
    }
}