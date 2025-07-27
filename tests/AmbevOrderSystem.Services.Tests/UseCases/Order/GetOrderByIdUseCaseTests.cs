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
    public class GetOrderByIdUseCaseTests : BaseTest
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<GetOrderByIdUseCase>> _loggerMock;
        private readonly GetOrderByIdUseCase _useCase;

        public GetOrderByIdUseCaseTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<GetOrderByIdUseCase>>();

            _useCase = new GetOrderByIdUseCase(
                _orderRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComPedidoExistente_DeveRetornarPedido()
        {
            // Arrange
            var command = _fixture.Create<GetOrderByIdCommand>();
            var order = _fixture.Create<CustomerOrder>();

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(command.OrderId))
                .ReturnsAsync(order);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(order.Id);

            _orderRepositoryMock.Verify(x => x.GetByIdAsync(command.OrderId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComPedidoNaoEncontrado_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetOrderByIdCommand>();

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(command.OrderId))
                .ReturnsAsync((CustomerOrder)null!);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Pedido não encontrado");

            _orderRepositoryMock.Verify(x => x.GetByIdAsync(command.OrderId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetOrderByIdCommand>();

            _orderRepositoryMock.Setup(x => x.GetByIdAsync(command.OrderId))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao buscar pedido");
        }
    }
}