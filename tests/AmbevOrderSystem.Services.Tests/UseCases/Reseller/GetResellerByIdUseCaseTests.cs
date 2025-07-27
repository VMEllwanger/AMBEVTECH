using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.UseCases.Reseller;
using Microsoft.Extensions.Logging;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Reseller
{
    public class GetResellerByIdUseCaseTests : BaseTest
    {
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<ILogger<GetResellerByIdUseCase>> _loggerMock;
        private readonly GetResellerByIdUseCase _useCase;

        public GetResellerByIdUseCaseTests()
        {
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _loggerMock = new Mock<ILogger<GetResellerByIdUseCase>>();

            _useCase = new GetResellerByIdUseCase(
                _resellerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaExistente_DeveRetornarRevenda()
        {
            // Arrange
            var command = _fixture.Create<GetResellerByIdCommand>();
            var reseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(reseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(reseller.Id);

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaNaoEncontrada_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetResellerByIdCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync((ResellerEntity)null!);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Revenda não encontrada");

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetResellerByIdCommand>();

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