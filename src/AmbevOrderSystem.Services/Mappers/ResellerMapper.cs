
using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Models.Responses.Reseller;

namespace AmbevOrderSystem.Services.Mappers
{
    /// <summary>
    /// Mapper manual para conversões relacionadas a Reseller
    /// </summary>
    public static class ResellerMapper
    {
        /// <summary>
        /// Converte CreateResellerCommand para entidade Reseller
        /// </summary>
        public static Reseller ToEntity(CreateResellerCommand command)
        {
            return new Reseller
            {
                RazaoSocial = command.RazaoSocial,
                NomeFantasia = command.NomeFantasia,
                Cnpj = command.Cnpj.Replace(".", "").Replace("/", "").Replace("-", ""),
                Email = command.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Phones = command.Phones?.Select(p => new Phone
                {
                    Number = p.Number,
                    IsPrimary = p.IsPrimary
                }).ToList() ?? new List<Phone>(),
                Contacts = command.Contacts?.Select(c => new Contact
                {
                    Name = c.Name,
                    IsPrimary = c.IsPrimary
                }).ToList() ?? new List<Contact>(),
                DeliveryAddresses = command.DeliveryAddresses?.Select(a => new DeliveryAddress
                {
                    Street = a.Street,
                    Number = a.Number,
                    Complement = a.Complement,
                    Neighborhood = a.Neighborhood,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    IsPrimary = a.IsPrimary
                }).ToList() ?? new List<DeliveryAddress>()
            };
        }

        /// <summary>
        /// Converte entidade Reseller para CreateResellerResponse
        /// </summary>
        public static ResellerResponse ToCreateResponse(Reseller entity)
        {
            return new ResellerResponse
            {
                Id = entity.Id,
                RazaoSocial = entity.RazaoSocial,
                NomeFantasia = entity.NomeFantasia,
                Cnpj = entity.Cnpj,
                Email = entity.Email,
                Phones = entity.Phones?.Select(x => new PhoneResponse
                {
                    Number = x.Number,
                    IsPrimary = x.IsPrimary
                }).ToList() ?? new List<PhoneResponse>(),
                Contacts = entity.Contacts?.Select(x => new ContactResponse
                {
                    Name = x.Name,
                    IsPrimary = x.IsPrimary
                }).ToList() ?? new List<ContactResponse>(),
                DeliveryAddresses = entity.DeliveryAddresses?.Select(x => new DeliveryAddressResponse
                {
                    City = x.City,
                    Complement = x.Complement,
                    Neighborhood = x.Neighborhood,
                    Number = x.Number,
                    State = x.State,
                    Street = x.Street,
                    ZipCode = x.ZipCode,
                    IsPrimary = x.IsPrimary
                }).ToList() ?? new List<DeliveryAddressResponse>(),
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedAt
            };
        }



        /// <summary>
        /// Atualiza entidade Reseller com dados do UpdateResellerCommand
        /// </summary>
        public static void UpdateEntity(Reseller entity, UpdateResellerCommand command)
        {
            entity.RazaoSocial = command.RazaoSocial;
            entity.NomeFantasia = command.NomeFantasia;
            entity.Cnpj = command.Cnpj;
            entity.Email = command.Email;
            entity.UpdatedAt = DateTime.UtcNow;

            entity.Phones = command.Phones?.Select(p => new Phone
            {
                Number = p.Number,
                IsPrimary = p.IsPrimary
            }).ToList() ?? new List<Phone>();

            entity.Contacts = command.Contacts?.Select(c => new Contact
            {
                Name = c.Name,
                IsPrimary = c.IsPrimary
            }).ToList() ?? new List<Contact>();

            entity.DeliveryAddresses = command.DeliveryAddresses?.Select(a => new DeliveryAddress
            {
                Street = a.Street,
                Number = a.Number,
                Complement = a.Complement,
                Neighborhood = a.Neighborhood,
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
                IsPrimary = a.IsPrimary
            }).ToList() ?? new List<DeliveryAddress>();
        }

        /// <summary>
        /// Converte entidade Reseller para ResellerDto (para compatibilidade com services existentes)
        /// </summary>
        public static ResellerDto MapToResellerDto(Reseller reseller)
        {
            return new ResellerDto
            {
                Id = reseller.Id,
                Cnpj = reseller.Cnpj,
                RazaoSocial = reseller.RazaoSocial,
                NomeFantasia = reseller.NomeFantasia,
                Email = reseller.Email,
                Phones = reseller.Phones.Select(p => new PhoneDto
                {
                    Id = p.Id,
                    Number = p.Number,
                    IsPrimary = p.IsPrimary
                }).ToList(),
                Contacts = reseller.Contacts.Select(c => new ContactDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsPrimary = c.IsPrimary
                }).ToList(),
                DeliveryAddresses = reseller.DeliveryAddresses.Select(a => new DeliveryAddressDto
                {
                    Id = a.Id,
                    Street = a.Street,
                    Number = a.Number,
                    Complement = a.Complement,
                    Neighborhood = a.Neighborhood,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    IsPrimary = a.IsPrimary
                }).ToList(),
                CreatedAt = reseller.CreatedAt,
                UpdatedAt = reseller.UpdatedAt
            };
        }

        public static List<ResellerResponse> ToGetResponseList(IEnumerable<Reseller> resellers)
        {
            return resellers.Select(reseller => ToCreateResponse(reseller)).ToList();
        }
    }
}