namespace AmbevOrderSystem.Services.Interfaces
{
    public interface IValidationService
    {
        bool IsValidCnpj(string cnpj);
        bool IsValidEmail(string email);
        bool IsValidPhone(string phone);
        bool IsValidCpf(string cpf);
        bool IsValidCep(string cep);
    }
}