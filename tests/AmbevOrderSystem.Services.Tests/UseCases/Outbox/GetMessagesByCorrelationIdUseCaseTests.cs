using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Outbox;
using AmbevOrderSystem.Services.UseCases.Outbox;
using Microsoft.Extensions.Logging;

namespace AmbevOrderSystem.Services.Tests.UseCases.Outbox
{
    public class GetMessagesByCorrelationIdUseCaseTests
    {
        private readonly Mock<IOutboxService> _outboxServiceMock;
        private readonly Mock<ILogger<GetMessagesByCorrelationIdUseCase>> _loggerMock;
        private readonly GetMessagesByCorrelationIdUseCase _useCase;

        public GetMessagesByCorrelationIdUseCaseTests()
        {
            _outboxServiceMock = new Mock<IOutboxService>();
            _loggerMock = new Mock<ILogger<GetMessagesByCorrelationIdUseCase>>();
            _useCase = new GetMessagesByCorrelationIdUseCase(_outboxServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_WhenSuccessful_ShouldReturnMessages()
        {
            // Arrange
            var correlationId = "test-correlation-id";
            var command = new GetMessagesByCorrelationIdCommand(correlationId);
            var expectedMessages = new List<OutboxMessage>
            {
                new OutboxMessage
                {
                    Id = 1,
                    Type = "AmbevOrderSubmission",
                    Status = "Completed",
                    CorrelationId = correlationId,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _outboxServiceMock.Setup(x => x.GetMessagesByCorrelationIdAsync(correlationId))
                .ReturnsAsync(expectedMessages);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Messages.Should().HaveCount(1);
            result.Data.Messages.Should().BeEquivalentTo(expectedMessages);
            result.Data.ResponseId.Should().NotBeNullOrEmpty();
            result.Data.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));

            _outboxServiceMock.Verify(x => x.GetMessagesByCorrelationIdAsync(correlationId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenServiceThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var correlationId = "test-correlation-id";
            var command = new GetMessagesByCorrelationIdCommand(correlationId);
            var exception = new Exception("Service error");

            _outboxServiceMock.Setup(x => x.GetMessagesByCorrelationIdAsync(correlationId))
                .ThrowsAsync(exception);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno ao obter mensagens");
            result.Data.Should().BeNull();

            _outboxServiceMock.Verify(x => x.GetMessagesByCorrelationIdAsync(correlationId), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_WhenNoMessagesFound_ShouldReturnEmptyList()
        {
            // Arrange
            var correlationId = "test-correlation-id";
            var command = new GetMessagesByCorrelationIdCommand(correlationId);
            var expectedMessages = new List<OutboxMessage>();

            _outboxServiceMock.Setup(x => x.GetMessagesByCorrelationIdAsync(correlationId))
                .ReturnsAsync(expectedMessages);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data!.Messages.Should().BeEmpty();
            result.Data.ResponseId.Should().NotBeNullOrEmpty();

            _outboxServiceMock.Verify(x => x.GetMessagesByCorrelationIdAsync(correlationId), Times.Once);
        }
    }
}