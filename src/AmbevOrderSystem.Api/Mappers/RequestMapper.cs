using AmbevOrderSystem.Api.Resquet;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Commands.Reseller;

namespace AmbevOrderSystem.Api.Mappers
{
    /// <summary>
    /// Mapper manual para conversões de DTOs da API para Commands
    /// </summary>
    public static class RequestMapper
    {
        /// <summary>
        /// Converte CreateResellerRequest para CreateResellerCommand
        /// </summary>
        public static CreateResellerCommand ToCommand(CreateResellerRequest request)
        {
            return new CreateResellerCommand
            {
                Cnpj = request.Cnpj,
                RazaoSocial = request.RazaoSocial,
                NomeFantasia = request.NomeFantasia,
                Email = request.Email,
                Phones = request.Phones.Select(p => new PhoneCommand
                {
                    Number = p.Number,
                    IsPrimary = p.IsPrimary
                }).ToList(),
                Contacts = request.Contacts.Select(c => new ContactCommand
                {
                    Name = c.Name,
                    IsPrimary = c.IsPrimary
                }).ToList(),
                DeliveryAddresses = request.DeliveryAddresses.Select(a => new DeliveryAddressCommand
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
        }

        /// <summary>
        /// Converte UpdateResellerRequest para UpdateResellerCommand
        /// </summary>
        public static UpdateResellerCommand ToCommand(UpdateResellerRequest request, int id)
        {
            return new UpdateResellerCommand
            {
                Id = id,
                Cnpj = request.Cnpj,
                RazaoSocial = request.RazaoSocial,
                NomeFantasia = request.NomeFantasia,
                Email = request.Email,
                Phones = request.Phones.Select(p => new PhoneCommand
                {
                    Number = p.Number,
                    IsPrimary = p.IsPrimary
                }).ToList(),
                Contacts = request.Contacts.Select(c => new ContactCommand
                {
                    Name = c.Name,
                    IsPrimary = c.IsPrimary
                }).ToList(),
                DeliveryAddresses = request.DeliveryAddresses.Select(a => new DeliveryAddressCommand
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
        }

        /// <summary>
        /// Converte CreateCustomerOrderRequest para CreateCustomerOrderCommand
        /// </summary>
        public static CreateCustomerOrderCommand ToCommand(CreateCustomerOrderRequest request, int resellerId)
        {
            return new CreateCustomerOrderCommand
            {
                ResellerId = resellerId,
                CustomerIdentification = request.CustomerIdentification,
                Items = request.Items.Select(i => new OrderItemCommand
                {
                    ProductSku = i.ProductSku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };
        }
    }
}