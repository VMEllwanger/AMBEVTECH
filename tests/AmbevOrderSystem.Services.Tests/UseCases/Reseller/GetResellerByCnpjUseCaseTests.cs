using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.UseCases.Reseller;
using Microsoft.Extensions.Logging;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Reseller
{
    public class GetResellerByCnpjUseCaseTests : BaseTest
    {
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<ILogger<GetResellerByCnpjUseCase>> _loggerMock;
        private readonly GetResellerByCnpjUseCase _useCase;

        public GetResellerByCnpjUseCaseTests()
        {
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _loggerMock = new Mock<ILogger<GetResellerByCnpjUseCase>>();

            _useCase = new GetResellerByCnpjUseCase(
                _resellerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaExistente_DeveRetornarRevenda()
        {
            // Arrange
            var command = _fixture.Create<GetResellerByCnpjCommand>();
            var reseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
                .ReturnsAsync(reseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(reseller.Id);

            _resellerRepositoryMock.Verify(x => x.GetByCnpjAsync(command.Cnpj), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaNaoEncontrada_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetResellerByCnpjCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
                .ReturnsAsync((ResellerEntity)null!);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Revenda não encontrada");

            _resellerRepositoryMock.Verify(x => x.GetByCnpjAsync(command.Cnpj), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetResellerByCnpjCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
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