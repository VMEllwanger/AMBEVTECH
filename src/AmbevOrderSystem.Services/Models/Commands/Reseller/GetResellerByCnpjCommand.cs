namespace AmbevOrderSystem.Services.Models.Commands.Reseller
{
    /// <summary>
    /// Comando para obter revenda por CNPJ
    /// </summary>
    public class GetResellerByCnpjCommand : BaseCommand
    {
        public string Cnpj { get; set; } = string.Empty;
    }
}