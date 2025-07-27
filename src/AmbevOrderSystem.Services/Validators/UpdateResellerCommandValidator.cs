using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using FluentValidation;

namespace AmbevOrderSystem.Services.Validators
{
    /// <summary>
    /// Validator para UpdateResellerCommand
    /// </summary>
    public class UpdateResellerCommandValidator : AbstractValidator<UpdateResellerCommand>
    {
        private readonly IValidationService _validationService;

        public UpdateResellerCommandValidator(IValidationService validationService)
        {
            _validationService = validationService;
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("ID deve ser maior que zero");

            RuleFor(x => x.RazaoSocial)
                .NotEmpty()
                .WithMessage("Razão Social é obrigatória")
                .MaximumLength(100)
                .WithMessage("Razão Social deve ter no máximo 100 caracteres");

            RuleFor(x => x.NomeFantasia)
                .NotEmpty()
                .WithMessage("Nome Fantasia é obrigatório")
                .MaximumLength(100)
                .WithMessage("Nome Fantasia deve ter no máximo 100 caracteres");

            RuleFor(x => x.Cnpj)
                .NotEmpty()
                .WithMessage("CNPJ é obrigatório")
                .Must(cnpj => _validationService.IsValidCnpj(cnpj))
                .WithMessage("CNPJ deve ser válido");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email é obrigatório")
                .Must(email => _validationService.IsValidEmail(email))
                .WithMessage("Email deve ter formato válido")
                .MaximumLength(150)
                .WithMessage("Email deve ter no máximo 150 caracteres");

            RuleFor(x => x.Phones)
                .Must(phones => phones == null || phones.All(p => _validationService.IsValidPhone(p.Number)))
                .WithMessage("Todos os telefones devem estar em formato válido");

            RuleFor(x => x.Contacts)
                .NotEmpty()
                .WithMessage("Deve haver pelo menos um contato")
                .Must(contacts => contacts.Any(c => c.IsPrimary))
                .WithMessage("Deve haver pelo menos um contato principal")
                .Must(contacts => contacts.All(c => !string.IsNullOrEmpty(c.Name)))
                .WithMessage("Todos os contatos devem ter nome válido");

            RuleFor(x => x.DeliveryAddresses)
                .NotEmpty()
                .WithMessage("Deve haver pelo menos um endereço de entrega")
                .Must(addresses => addresses.Any(a => a.IsPrimary))
                .WithMessage("Deve haver pelo menos um endereço principal")
                .Must(addresses => addresses.All(a => IsValidAddress(a)))
                .WithMessage("Todos os endereços devem ter dados válidos");
        }



        private bool IsValidAddress(DeliveryAddressCommand address)
        {
            return !string.IsNullOrEmpty(address.Street) &&
                   !string.IsNullOrEmpty(address.Number) &&
                   !string.IsNullOrEmpty(address.Neighborhood) &&
                   !string.IsNullOrEmpty(address.City) &&
                   !string.IsNullOrEmpty(address.State) &&
                   address.State.Length == 2 &&
                   !string.IsNullOrEmpty(address.ZipCode) &&
                   _validationService.IsValidCep(address.ZipCode);
        }
    }
}