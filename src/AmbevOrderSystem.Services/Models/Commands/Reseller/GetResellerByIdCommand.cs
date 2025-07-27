namespace AmbevOrderSystem.Services.Models.Commands.Reseller
{
    /// <summary>
    /// Comando para obter revenda por ID
    /// </summary>
    public class GetResellerByIdCommand : BaseCommand
    {
        public int Id { get; set; }
    }
}