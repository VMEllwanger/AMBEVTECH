namespace AmbevOrderSystem.Services.Models.Responses.Reseller
{
    /// <summary>
    /// Comando para telefone
    /// </summary>
    public class PhoneResponse
    {
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

}