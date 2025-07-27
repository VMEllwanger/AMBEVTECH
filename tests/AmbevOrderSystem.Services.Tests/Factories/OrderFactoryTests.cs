using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Factories;

namespace AmbevOrderSystem.Services.Tests.Factories
{
    public class OrderFactoryTests : BaseTest
    {
        private readonly OrderFactory _factory;

        public OrderFactoryTests()
        {
            _factory = new OrderFactory();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarPedidoCorretamente()
        {
            // Arrange
            var resellerId = 1;
            var request = CreateValidRequest();

            // Act
            var result = await _factory.CreateAsync(resellerId, request);

            // Assert
            result.Should().NotBeNull();
            result.CustomerIdentification.Should().Be(request.CustomerIdentification);
            result.ResellerId.Should().Be(resellerId);
            result.Items.Should().HaveCount(request.Items.Count);
            result.Items.Should().AllSatisfy(item => item.Should().NotBeNull());
        }

        [Fact]
        public async Task CreateAsync_ComItensValidos_DeveMapearItensCorretamente()
        {
            // Arrange
            var resellerId = 1;
            var request = CreateValidRequest();

            // Act
            var result = await _factory.CreateAsync(resellerId, request);

            // Assert
            result.Items.Should().HaveCount(2);

            var firstItem = result.Items.First();
            firstItem.ProductSku.Should().Be(request.Items[0].ProductSku);
            firstItem.ProductName.Should().Be(request.Items[0].ProductName);
            firstItem.Quantity.Should().Be(request.Items[0].Quantity);
            firstItem.UnitPrice.Should().Be(request.Items[0].UnitPrice);

            var secondItem = result.Items.Last();
            secondItem.ProductSku.Should().Be(request.Items[1].ProductSku);
            secondItem.ProductName.Should().Be(request.Items[1].ProductName);
            secondItem.Quantity.Should().Be(request.Items[1].Quantity);
            secondItem.UnitPrice.Should().Be(request.Items[1].UnitPrice);
        }

        [Fact]
        public async Task CreateAsync_ComListaItensVazia_DeveCriarPedidoSemItens()
        {
            // Arrange
            var resellerId = 1;
            var request = new CreateCustomerOrderRequest
            {
                CustomerIdentification = "CLI001",
                Items = new List<OrderItemDto>()
            };

            // Act
            var result = await _factory.CreateAsync(resellerId, request);

            // Assert
            result.Should().NotBeNull();
            result.CustomerIdentification.Should().Be(request.CustomerIdentification);
            result.ResellerId.Should().Be(resellerId);
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task CreateAsync_ComDiferentesResellerIds_DeveUsarIdCorreto()
        {
            // Arrange
            var resellerId1 = 1;
            var resellerId2 = 999;
            var request = CreateValidRequest();

            // Act
            var result1 = await _factory.CreateAsync(resellerId1, request);
            var result2 = await _factory.CreateAsync(resellerId2, request);

            // Assert
            result1.ResellerId.Should().Be(resellerId1);
            result2.ResellerId.Should().Be(resellerId2);
        }

        private CreateCustomerOrderRequest CreateValidRequest()
        {
            return new CreateCustomerOrderRequest
            {
                CustomerIdentification = "CLI001",
                Items = new List<OrderItemDto>
                {
                    new() { ProductSku = "SKU001", ProductName = "Produto 1", Quantity = 10, UnitPrice = 10.50m },
                    new() { ProductSku = "SKU002", ProductName = "Produto 2", Quantity = 5, UnitPrice = 20.00m }
                }
            };
        }
    }
}