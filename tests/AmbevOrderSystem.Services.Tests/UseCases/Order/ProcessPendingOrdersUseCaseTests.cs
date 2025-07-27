using AmbevOrderSystem.Infrastructure.DTOs;
using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.UseCases.Order;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Order
{
    public class ProcessPendingOrdersUseCaseTests : BaseTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<IAmbevApiService> _ambevApiServiceMock;
        private readonly Mock<ILogger<ProcessPendingOrdersUseCase>> _loggerMock;
        private readonly ProcessPendingOrdersUseCase _useCase;

        public ProcessPendingOrdersUseCaseTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _ambevApiServiceMock = new Mock<IAmbevApiService>();
            _loggerMock = new Mock<ILogger<ProcessPendingOrdersUseCase>>();

            _useCase = new ProcessPendingOrdersUseCase(
                _orderRepositoryMock.Object,
                _resellerRepositoryMock.Object,
                _ambevApiServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComPedidosPendentesEQuantidadeSuficiente_DeveProcessarComSucesso()
        {
            // Arrange
            var command = _fixture.Create<ProcessPendingOrdersCommand>();
            var resellers = _fixture.CreateMany<ResellerEntity>(1).ToList();
            var pendingOrders = new List<CustomerOrder>
            {
                new() {
                    ResellerId = resellers[0].Id,
                    Status = OrderStatus.Pending,
                    Reseller = resellers[0],
                    Items = new List<OrderItem> {
                        new() { Quantity = 600, ProductSku = "SKU1", ProductName = "Product1", UnitPrice = 10 }
                    }
                },
                new() {
                    ResellerId = resellers[0].Id,
                    Status = OrderStatus.Pending,
                    Reseller = resellers[0],
                    Items = new List<OrderItem> {
                        new() { Quantity = 500, ProductSku = "SKU2", ProductName = "Product2", UnitPrice = 15 }
                    }
                }
            };

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(resellers);

            _orderRepositoryMock.Setup(x => x.GetPendingOrdersAsync())
                .ReturnsAsync(pendingOrders);

            _orderRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CustomerOrder>()))
                .ReturnsAsync((CustomerOrder order) => order);

            _ambevApiServiceMock.Setup(x => x.SubmitOrderAsync(It.IsAny<AmbevOrderRequest>()))
                .ReturnsAsync(new AmbevOrderResponse { OrderNumber = "AMB-123" });

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProcessedOrders.Should().Be(2);
            result.Value.SentToAmbev.Should().Be(2);

            _ambevApiServiceMock.Verify(x => x.SubmitOrderAsync(It.IsAny<AmbevOrderRequest>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComQuantidadeInsuficiente_NaoDeveEnviarParaAmbev()
        {
            // Arrange
            var command = _fixture.Create<ProcessPendingOrdersCommand>();
            var resellers = _fixture.CreateMany<ResellerEntity>(1).ToList();
            var pendingOrders = new List<CustomerOrder>
            {
                new() {
                    ResellerId = resellers[0].Id,
                    Status = OrderStatus.Pending,
                    Reseller = resellers[0],
                    Items = new List<OrderItem> {
                        new() { Quantity = 300, ProductSku = "SKU1", ProductName = "Product1", UnitPrice = 10 }
                    }
                },
                new() {
                    ResellerId = resellers[0].Id,
                    Status = OrderStatus.Pending,
                    Reseller = resellers[0],
                    Items = new List<OrderItem> {
                        new() { Quantity = 200, ProductSku = "SKU2", ProductName = "Product2", UnitPrice = 15 }
                    }
                }
            };

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(resellers);

            _orderRepositoryMock.Setup(x => x.GetPendingOrdersAsync())
                .ReturnsAsync(pendingOrders);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProcessedOrders.Should().Be(2);
            result.Value.SentToAmbev.Should().Be(0);

            _ambevApiServiceMock.Verify(x => x.SubmitOrderAsync(It.IsAny<AmbevOrderRequest>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecaoNaApiAmbev_DeveMarcarComoRetry()
        {
            // Arrange
            var command = _fixture.Create<ProcessPendingOrdersCommand>();
            var resellers = _fixture.CreateMany<ResellerEntity>(1).ToList();
            var pendingOrders = new List<CustomerOrder>
            {
                new() {
                    ResellerId = resellers[0].Id,
                    Status = OrderStatus.Pending,
                    Reseller = resellers[0],
                    Items = new List<OrderItem> {
                        new() { Quantity = 1000, ProductSku = "SKU1", ProductName = "Product1", UnitPrice = 10 }
                    }
                }
            };

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(resellers);

            _orderRepositoryMock.Setup(x => x.GetPendingOrdersAsync())
                .ReturnsAsync(pendingOrders);

            _orderRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CustomerOrder>()))
                .ReturnsAsync((CustomerOrder order) => order);

            _ambevApiServiceMock.Setup(x => x.SubmitOrderAsync(It.IsAny<AmbevOrderRequest>()))
                .ThrowsAsync(new Exception("Erro na API"));

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProcessedOrders.Should().Be(1);
            result.Value.SentToAmbev.Should().Be(0);
            result.Value.FailedOrders.Should().Be(1);

            _orderRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<CustomerOrder>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecaoGeral_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<ProcessPendingOrdersCommand>();

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao processar pedidos");
        }

        [Fact]
        public async Task ExecuteAsync_ComListaVazia_DeveRetornarSucesso()
        {
            // Arrange
            var command = _fixture.Create<ProcessPendingOrdersCommand>();

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ResellerEntity>());

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ProcessedOrders.Should().Be(0);
            result.Value.SentToAmbev.Should().Be(0);
            result.Value.FailedOrders.Should().Be(0);
        }
    }
}