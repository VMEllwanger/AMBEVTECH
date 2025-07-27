namespace AmbevOrderSystem.Services.Models.Commands.Reseller
{
    /// <summary>
    /// Comando para criação de uma nova revenda
    /// </summary>
    public class CreateResellerCommand : BaseCommand
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneCommand> Phones { get; set; } = new();
        public List<ContactCommand> Contacts { get; set; } = new();
        public List<DeliveryAddressCommand> DeliveryAddresses { get; set; } = new();
    }

    /// <summary>
    /// Comando para telefone
    /// </summary>
    public class PhoneCommand
    {
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// Comando para contato
    /// </summary>
    public class ContactCommand
    {
        public string Name { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

    /// <summary>
    /// Comando para endereço de entrega
    /// </summary>
    public class DeliveryAddressCommand
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