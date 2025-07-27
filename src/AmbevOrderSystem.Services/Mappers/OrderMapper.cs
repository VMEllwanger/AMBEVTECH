using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Models.Responses.Order;

namespace AmbevOrderSystem.Services.Mappers
{
    /// <summary>
    /// Mapper manual para conversões relacionadas a Order
    /// </summary>
    public static class OrderMapper
    {
        /// <summary>
        /// Converte entidade CustomerOrder para CreateCustomerOrderResponse
        /// </summary>
        public static CreateCustomerOrderResponse ToCreateResponse(CustomerOrder entity)
        {
            return new CreateCustomerOrderResponse
            {
                OrderId = entity.Id,
                CustomerIdentification = entity.CustomerIdentification,
                Items = entity.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    ProductSku = i.ProductSku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Status = entity.Status.ToString(),
                OrderCreatedAt = entity.CreatedAt
            };
        }

        /// <summary>
        /// Converte entidade CustomerOrder para GetOrderResponse
        /// </summary>
        public static GetOrderResponse ToGetResponse(CustomerOrder entity)
        {
            return new GetOrderResponse
            {
                Id = entity.Id,
                ResellerId = entity.ResellerId,
                CustomerIdentification = entity.CustomerIdentification,
                Items = entity.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    ProductSku = i.ProductSku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Status = entity.Status.ToString(),
                AmbevOrderNumber = entity.AmbevOrderNumber,
                OrderCreatedAt = entity.CreatedAt
            };
        }

        /// <summary>
        /// Converte lista de entidades CustomerOrder para lista de GetOrderResponse
        /// </summary>
        public static List<GetOrderResponse> ToGetResponseList(IEnumerable<CustomerOrder> entities)
        {
            return entities.Select(ToGetResponse).ToList();
        }

        /// <summary>
        /// Converte entidade CustomerOrder para CustomerOrderDto (para compatibilidade com services existentes)
        /// </summary>
        public static CustomerOrderDto MapToCustomerOrderDto(CustomerOrder entity)
        {
            return new CustomerOrderDto
            {
                Id = entity.Id,
                CustomerIdentification = entity.CustomerIdentification,
                ResellerId = entity.ResellerId,
                ResellerName = entity.Reseller?.NomeFantasia ?? string.Empty,
                Items = entity.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductSku = i.ProductSku,
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList(),
                Status = entity.Status.ToString(),
                CreatedAt = entity.CreatedAt,
                AmbevOrderNumber = entity.AmbevOrderNumber
            };
        }

        /// <summary>
        /// Converte lista de entidades CustomerOrder para lista de CustomerOrderDto
        /// </summary>
        public static List<CustomerOrderDto> MapToCustomerOrderDtoList(IEnumerable<CustomerOrder> entities)
        {
            return entities.Select(MapToCustomerOrderDto).ToList();
        }
    }
}