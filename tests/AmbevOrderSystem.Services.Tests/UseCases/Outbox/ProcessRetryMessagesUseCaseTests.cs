using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.UseCases.Outbox;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.Tests.UseCases.Outbox
{
    public class ProcessRetryMessagesUseCaseTests
    {
        private readonly Mock<IOutboxService> _outboxServiceMock;
        private readonly Mock<ILogger<ProcessRetryMessagesUseCase>> _loggerMock;
        private readonly ProcessRetryMessagesUseCase _useCase;

        public ProcessRetryMessagesUseCaseTests()
        {
            _outboxServiceMock = new Mock<IOutboxService>();
            _loggerMock = new Mock<ILogger<ProcessRetryMessagesUseCase>>();
            _useCase = new ProcessRetryMessagesUseCase(_outboxServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenSuccessful_ShouldReturnSuccessMessage()
        {
            // Arrange
            var command = new ProcessRetryMessagesCommand();

            _outboxServiceMock.Setup(x => x.ProcessRetryMessagesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Message.Should().Be("Processamento de mensagens de retry iniciado");
            result.Data.ResponseId.Should().NotBeNullOrEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            _outboxServiceMock.Verify(x => x.ProcessRetryMessagesAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenServiceThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var command = new ProcessRetryMessagesCommand();
            var exception = new Exception("Service error");

            _outboxServiceMock.Setup(x => x.ProcessRetryMessagesAsync())
                .ThrowsAsync(exception);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao processar mensagens de retry");
            result.Data.Should().BeNull();

            _outboxServiceMock.Verify(x => x.ProcessRetryMessagesAsync(), Times.Once);
        }
    }
}