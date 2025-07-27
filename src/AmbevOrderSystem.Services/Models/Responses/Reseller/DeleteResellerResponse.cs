namespace AmbevOrderSystem.Services.Models.Responses.Reseller
{
    /// <summary>
    /// Resposta para deletar uma revenda
    /// </summary>
    public class DeleteResellerResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
    }
}