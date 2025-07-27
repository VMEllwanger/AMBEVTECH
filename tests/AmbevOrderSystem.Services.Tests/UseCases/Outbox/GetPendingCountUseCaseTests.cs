using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.UseCases.Outbox;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.Tests.UseCases.Outbox
{
    public class GetPendingCountUseCaseTests
    {
        private readonly Mock<IOutboxService> _outboxServiceMock;
        private readonly Mock<ILogger<GetPendingCountUseCase>> _loggerMock;
        private readonly GetPendingCountUseCase _useCase;

        public GetPendingCountUseCaseTests()
        {
            _outboxServiceMock = new Mock<IOutboxService>();
            _loggerMock = new Mock<ILogger<GetPendingCountUseCase>>();
            _useCase = new GetPendingCountUseCase(_outboxServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenSuccessful_ShouldReturnPendingCount()
        {
            // Arrange
            var command = new GetPendingCountCommand();
            var expectedCount = 5;

            _outboxServiceMock.Setup(x => x.GetPendingCountAsync())
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.PendingCount.Should().Be(expectedCount);
            result.Data.ResponseId.Should().NotBeNullOrEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            _outboxServiceMock.Verify(x => x.GetPendingCountAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenServiceThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var command = new GetPendingCountCommand();
            var exception = new Exception("Service error");

            _outboxServiceMock.Setup(x => x.GetPendingCountAsync())
                .ThrowsAsync(exception);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao obter contagem de mensagens pendentes");
            result.Data.Should().BeNull();

            _outboxServiceMock.Verify(x => x.GetPendingCountAsync(), Times.Once);
        }
    }
}