using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.UseCases.Reseller;
using Microsoft.Extensions.Logging;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Reseller
{
    public class CreateResellerUseCaseTests : BaseTest
    {
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<ILogger<CreateResellerUseCase>> _loggerMock;
        private readonly CreateResellerUseCase _useCase;

        public CreateResellerUseCaseTests()
        {
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _loggerMock = new Mock<ILogger<CreateResellerUseCase>>();

            _useCase = new CreateResellerUseCase(
                _resellerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComDadosValidos_DeveCriarRevendaComSucesso()
        {
            // Arrange
            var command = _fixture.Create<CreateResellerCommand>();
            var createdReseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
                .ReturnsAsync((ResellerEntity)null!);

            _resellerRepositoryMock.Setup(x => x.AddAsync(It.IsAny<ResellerEntity>()))
                .ReturnsAsync(createdReseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(createdReseller.Id);

            _resellerRepositoryMock.Verify(x => x.GetByCnpjAsync(command.Cnpj), Times.Once);
            _resellerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ResellerEntity>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComCnpjJaExistente_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<CreateResellerCommand>();
            var existingReseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
                .ReturnsAsync(existingReseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("CNPJ já cadastrado no sistema");

            _resellerRepositoryMock.Verify(x => x.GetByCnpjAsync(command.Cnpj), Times.Once);
            _resellerRepositoryMock.Verify(x => x.AddAsync(It.IsAny<ResellerEntity>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<CreateResellerCommand>();

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