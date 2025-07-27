namespace AmbevOrderSystem.Services.Models.Responses.Reseller
{
    /// <summary>
    /// Comando para endereço de entrega
    /// </summary>
    public class DeliveryAddressResponse
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