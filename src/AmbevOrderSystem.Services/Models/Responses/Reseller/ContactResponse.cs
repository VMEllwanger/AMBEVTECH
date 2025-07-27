namespace AmbevOrderSystem.Services.Models.Responses.Reseller
{
    /// <summary>
    /// Comando para contato
    /// </summary>
    public class ContactResponse
    {
        public string Name { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } = false;
    }

}