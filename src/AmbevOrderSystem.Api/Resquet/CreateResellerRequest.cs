namespace AmbevOrderSystem.Api.Resquet
{
    /// <summary>
    /// DTO para requisição de criação de reseller
    /// </summary>
    public class CreateResellerRequest
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneRequest> Phones { get; set; } = new();
        public List<ContactRequest> Contacts { get; set; } = new();
        public List<DeliveryAddressRequest> DeliveryAddresses { get; set; } = new();
    }

    /// <summary>
    /// DTO para telefone
    /// </summary>
    public class PhoneRequest
    {
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// DTO para contato
    /// </summary>
    public class ContactRequest
    {
        public string Name { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// DTO para endereço de entrega
    /// </summary>
    public class DeliveryAddressRequest
    {
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Complement { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }
}