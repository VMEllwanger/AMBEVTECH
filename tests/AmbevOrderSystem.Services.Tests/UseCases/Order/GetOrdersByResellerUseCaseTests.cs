using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Order;
using AmbevOrderSystem.Services.Models.Responses.Order;
using AmbevOrderSystem.Services.UseCases.Order;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace AmbevOrderSystem.Services.Tests.UseCases.Order
{
    public class GetOrdersByResellerUseCaseTests : BaseTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<GetOrdersByResellerUseCase>> _loggerMock;
        private readonly GetOrdersByResellerUseCase _useCase;

        public GetOrdersByResellerUseCaseTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<GetOrdersByResellerUseCase>>();

            _useCase = new GetOrdersByResellerUseCase(
                _orderRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComPedidosExistentes_DeveRetornarListaDePedidos()
        {
            // Arrange
            var command = _fixture.Create<GetOrdersByResellerCommand>();
            var orders = _fixture.CreateMany<CustomerOrder>(3).ToList();

            _orderRepositoryMock.Setup(x => x.GetByResellerIdAsync(command.ResellerId))
                .ReturnsAsync(orders);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Orders.Should().HaveCount(3);

            _orderRepositoryMock.Verify(x => x.GetByResellerIdAsync(command.ResellerId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var command = _fixture.Create<GetOrdersByResellerCommand>();

            _orderRepositoryMock.Setup(x => x.GetByResellerIdAsync(command.ResellerId))
                .ReturnsAsync(new List<CustomerOrder>());

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Orders.Should().BeEmpty();

            _orderRepositoryMock.Verify(x => x.GetByResellerIdAsync(command.ResellerId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetOrdersByResellerCommand>();

            _orderRepositoryMock.Setup(x => x.GetByResellerIdAsync(command.ResellerId))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao buscar pedidos");
        }
    }
}