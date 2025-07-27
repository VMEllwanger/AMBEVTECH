using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.UseCases.Reseller;
using Microsoft.Extensions.Logging;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Reseller
{
    public class DeleteResellerUseCaseTests : BaseTest
    {
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<ILogger<DeleteResellerUseCase>> _loggerMock;
        private readonly DeleteResellerUseCase _useCase;

        public DeleteResellerUseCaseTests()
        {
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _loggerMock = new Mock<ILogger<DeleteResellerUseCase>>();

            _useCase = new DeleteResellerUseCase(
                _resellerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaExistente_DeveDeletarComSucesso()
        {
            // Arrange
            var command = _fixture.Create<DeleteResellerCommand>();
            var existingReseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(existingReseller);

            _resellerRepositoryMock.Setup(x => x.DeleteAsync(existingReseller))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(command.Id);
            result.Value.IsDeleted.Should().BeTrue();
            result.Value.Message.Should().Be("Revenda deletada com sucesso");

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
            _resellerRepositoryMock.Verify(x => x.DeleteAsync(existingReseller), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaNaoEncontrada_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<DeleteResellerCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync((ResellerEntity)null!);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Revenda não encontrada");

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
            _resellerRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<ResellerEntity>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<DeleteResellerCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Erro interno do servidor");
        }
    }
}