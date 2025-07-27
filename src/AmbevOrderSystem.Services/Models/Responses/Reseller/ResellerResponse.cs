namespace AmbevOrderSystem.Services.Models.Responses.Reseller
{
    /// <summary>
    /// Resposta para criação de uma nova revenda
    /// </summary>
    public class ResellerResponse : BaseResponse
    {
        public int Id { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneResponse> Phones { get; set; } = new();
        public List<ContactResponse> Contacts { get; set; } = new();
        public List<DeliveryAddressResponse> DeliveryAddresses { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

}