namespace AmbevOrderSystem.Api.Resquet
{
    /// <summary>
    /// DTO para requisição de atualização de reseller
    /// </summary>
    public class UpdateResellerRequest
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneRequest> Phones { get; set; } = new();
        public List<ContactRequest> Contacts { get; set; } = new();
        public List<DeliveryAddressRequest> DeliveryAddresses { get; set; } = new();
    }
}