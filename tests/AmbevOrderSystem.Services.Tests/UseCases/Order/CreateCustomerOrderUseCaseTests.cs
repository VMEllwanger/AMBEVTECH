using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Factories;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Responses.Order;
using AmbevOrderSystem.Services.UseCases.Order;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Order
{
    public class CreateCustomerOrderUseCaseTests : BaseTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<IOrderFactory> _orderFactoryMock;
        private readonly Mock<IAmbevApiService> _ambevApiServiceMock;
        private readonly Mock<ILogger<CreateCustomerOrderUseCase>> _loggerMock;
        private readonly CreateCustomerOrderUseCase _useCase;

        public CreateCustomerOrderUseCaseTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _orderFactoryMock = new Mock<IOrderFactory>();
            _ambevApiServiceMock = new Mock<IAmbevApiService>();
            _loggerMock = new Mock<ILogger<CreateCustomerOrderUseCase>>();

            _useCase = new CreateCustomerOrderUseCase(
                _orderRepositoryMock.Object,
                _resellerRepositoryMock.Object,
                _orderFactoryMock.Object,
                _ambevApiServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComDadosValidos_DeveCriarPedidoComSucesso()
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();
            var reseller = _fixture.Create<ResellerEntity>();
            var order = _fixture.Create<CustomerOrder>();
            var createdOrder = _fixture.Create<CustomerOrder>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ReturnsAsync(reseller);

            _orderFactoryMock.Setup(x => x.CreateAsync(command.ResellerId, It.IsAny<DTOs.CreateCustomerOrderRequest>()))
                .ReturnsAsync(order);

            _orderRepositoryMock.Setup(x => x.AddAsync(order))
                .ReturnsAsync(createdOrder);

            _orderRepositoryMock.Setup(x => x.GetByResellerIdAsync(command.ResellerId))
                .ReturnsAsync(new List<CustomerOrder>());

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.ResellerId), Times.Once);
            _orderFactoryMock.Verify(x => x.CreateAsync(command.ResellerId, It.IsAny<DTOs.CreateCustomerOrderRequest>()), Times.Once);
            _orderRepositoryMock.Verify(x => x.AddAsync(order), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaNaoEncontrada_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ReturnsAsync((ResellerEntity)null);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Revenda não encontrada");

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.ResellerId), Times.Once);
            _orderFactoryMock.Verify(x => x.CreateAsync(It.IsAny<int>(), It.IsAny<DTOs.CreateCustomerOrderRequest>()), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ExecuteAsync_ComIdentificacaoClienteInvalida_DeveRetornarFalha(string customerIdentification)
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();
            command.CustomerIdentification = customerIdentification;
            var reseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ReturnsAsync(reseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Identificação do cliente é obrigatória");
        }

        [Fact]
        public async Task ExecuteAsync_ComListaItensVazia_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();
            command.Items = new List<OrderItemCommand>();
            var reseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ReturnsAsync(reseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Deve haver pelo menos um item no pedido");
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao criar pedido");
        }

        [Fact]
        public async Task ExecuteAsync_ComQuantidadeMinimaAtingida_DeveEnviarParaAmbev()
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();
            var reseller = _fixture.Create<ResellerEntity>();
            var order = _fixture.Create<CustomerOrder>();
            var createdOrder = _fixture.Create<CustomerOrder>();

            createdOrder.Status = OrderStatus.Pending;
            createdOrder.Reseller = reseller;
            createdOrder.Items = new List<OrderItem>
            {
                new() { Quantity = 600, ProductSku = "SKU1", ProductName = "Product 1", UnitPrice = 10 },
                new() { Quantity = 400, ProductSku = "SKU2", ProductName = "Product 2", UnitPrice = 15 }
            };

            command.Items = new List<OrderItemCommand>
            {
                new() { Quantity = 600, ProductSku = "SKU1", ProductName = "Product 1", UnitPrice = 10 },
                new() { Quantity = 400, ProductSku = "SKU2", ProductName = "Product 2", UnitPrice = 15 }
            };

            var pendingOrders = new List<CustomerOrder>
            {
                createdOrder
            };

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ReturnsAsync(reseller);

            _orderFactoryMock.Setup(x => x.CreateAsync(command.ResellerId, It.IsAny<DTOs.CreateCustomerOrderRequest>()))
                .ReturnsAsync(order);

            _orderRepositoryMock.Setup(x => x.AddAsync(order))
                .ReturnsAsync(createdOrder);

            _orderRepositoryMock.Setup(x => x.GetByResellerIdAsync(command.ResellerId))
                .ReturnsAsync(pendingOrders);

            _ambevApiServiceMock.Setup(x => x.SubmitOrderAsync(It.IsAny<Infrastructure.DTOs.AmbevOrderRequest>()))
                .ReturnsAsync(new Infrastructure.DTOs.AmbevOrderResponse { OrderNumber = "AMB-123" });

            _orderRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CustomerOrder>()))
                .ReturnsAsync((CustomerOrder order) => order);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _ambevApiServiceMock.Verify(x => x.SubmitOrderAsync(It.IsAny<Infrastructure.DTOs.AmbevOrderRequest>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComQuantidadeInsuficiente_NaoDeveEnviarParaAmbev()
        {
            // Arrange
            var command = _fixture.Create<CreateCustomerOrderCommand>();
            var reseller = _fixture.Create<ResellerEntity>();
            var order = _fixture.Create<CustomerOrder>();
            var createdOrder = _fixture.Create<CustomerOrder>();

            command.Items = new List<OrderItemCommand>
            {
                new() { Quantity = 300 },
                new() { Quantity = 200 }
            };

            var pendingOrders = new List<CustomerOrder>
            {
                new() { Status = OrderStatus.Pending, Items = new List<OrderItem> { new() { Quantity = 300 } } },
                new() { Status = OrderStatus.Pending, Items = new List<OrderItem> { new() { Quantity = 200 } } }
            };

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.ResellerId))
                .ReturnsAsync(reseller);

            _orderFactoryMock.Setup(x => x.CreateAsync(command.ResellerId, It.IsAny<DTOs.CreateCustomerOrderRequest>()))
                .ReturnsAsync(order);

            _orderRepositoryMock.Setup(x => x.AddAsync(order))
                .ReturnsAsync(createdOrder);

            _orderRepositoryMock.Setup(x => x.GetByResellerIdAsync(command.ResellerId))
                .ReturnsAsync(pendingOrders);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            _ambevApiServiceMock.Verify(x => x.SubmitOrderAsync(It.IsAny<Infrastructure.DTOs.AmbevOrderRequest>()), Times.Never);
        }
    }
}