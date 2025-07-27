namespace AmbevOrderSystem.Services.Models.Commands.Reseller
{
    /// <summary>
    /// Comando para atualizar uma revenda
    /// </summary>
    public class UpdateResellerCommand : BaseCommand
    {
        public int Id { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneCommand> Phones { get; set; } = new();
        public List<ContactCommand> Contacts { get; set; } = new();
        public List<DeliveryAddressCommand> DeliveryAddresses { get; set; } = new();
    }
}