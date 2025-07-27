using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Interfaces;

namespace AmbevOrderSystem.Services.Factories
{
    public class ResellerFactory : IResellerFactory
    {
        public Task<Reseller> CreateAsync(CreateResellerRequest request)
        {
            var reseller = new Reseller
            {
                Cnpj = CleanCnpj(request.Cnpj),
                RazaoSocial = request.RazaoSocial,
                NomeFantasia = request.NomeFantasia,
                Email = request.Email,
                Phones = request.Phones.Select(p => new Phone
                {
                    Number = p.Number,
                    IsPrimary = p.IsPrimary
                }).ToList(),
                Contacts = request.Contacts.Select(c => new Contact
                {
                    Name = c.Name,
                    IsPrimary = c.IsPrimary
                }).ToList(),
                DeliveryAddresses = request.DeliveryAddresses.Select(a => new DeliveryAddress
                {
                    Street = a.Street,
                    Number = a.Number,
                    Complement = a.Complement,
                    Neighborhood = a.Neighborhood,
                    City = a.City,
                    State = a.State,
                    ZipCode = a.ZipCode,
                    IsPrimary = a.IsPrimary
                }).ToList()
            };

            return Task.FromResult(reseller);
        }

        public Task<Reseller> UpdateAsync(Reseller existingReseller, UpdateResellerRequest request)
        {
            existingReseller.RazaoSocial = request.RazaoSocial;
            existingReseller.NomeFantasia = request.NomeFantasia;
            existingReseller.Email = request.Email;
            existingReseller.UpdatedAt = DateTime.UtcNow;

            existingReseller.Phones.Clear();
            existingReseller.Contacts.Clear();
            existingReseller.DeliveryAddresses.Clear();

            existingReseller.Phones = request.Phones.Select(p => new Phone
            {
                Number = p.Number,
                IsPrimary = p.IsPrimary,
                ResellerId = existingReseller.Id
            }).ToList();

            existingReseller.Contacts = request.Contacts.Select(c => new Contact
            {
                Name = c.Name,
                IsPrimary = c.IsPrimary,
                ResellerId = existingReseller.Id
            }).ToList();

            existingReseller.DeliveryAddresses = request.DeliveryAddresses.Select(a => new DeliveryAddress
            {
                Street = a.Street,
                Number = a.Number,
                Complement = a.Complement,
                Neighborhood = a.Neighborhood,
                City = a.City,
                State = a.State,
                ZipCode = a.ZipCode,
                IsPrimary = a.IsPrimary,
                ResellerId = existingReseller.Id
            }).ToList();

            return Task.FromResult(existingReseller);
        }

        private static string CleanCnpj(string cnpj)
        {
            return cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
        }
    }
}