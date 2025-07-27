using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.UseCases.Outbox;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.Tests.UseCases.Outbox
{
    public class ProcessPendingMessagesUseCaseTests
    {
        private readonly Mock<IOutboxService> _outboxServiceMock;
        private readonly Mock<ILogger<ProcessPendingMessagesUseCase>> _loggerMock;
        private readonly ProcessPendingMessagesUseCase _useCase;

        public ProcessPendingMessagesUseCaseTests()
        {
            _outboxServiceMock = new Mock<IOutboxService>();
            _loggerMock = new Mock<ILogger<ProcessPendingMessagesUseCase>>();
            _useCase = new ProcessPendingMessagesUseCase(_outboxServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenSuccessful_ShouldReturnSuccessMessage()
        {
            // Arrange
            var command = new ProcessPendingMessagesCommand();

            _outboxServiceMock.Setup(x => x.ProcessPendingMessagesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Message.Should().Be("Processamento de mensagens pendentes iniciado");
            result.Data.ResponseId.Should().NotBeNullOrEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            _outboxServiceMock.Verify(x => x.ProcessPendingMessagesAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenServiceThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var command = new ProcessPendingMessagesCommand();
            var exception = new Exception("Service error");

            _outboxServiceMock.Setup(x => x.ProcessPendingMessagesAsync())
                .ThrowsAsync(exception);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao processar mensagens pendentes");
            result.Data.Should().BeNull();

            _outboxServiceMock.Verify(x => x.ProcessPendingMessagesAsync(), Times.Once);
        }
    }
}