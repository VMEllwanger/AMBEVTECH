using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.UseCases.Reseller;
using Microsoft.Extensions.Logging;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Reseller
{
    public class UpdateResellerUseCaseTests : BaseTest
    {
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<ILogger<UpdateResellerUseCase>> _loggerMock;
        private readonly UpdateResellerUseCase _useCase;

        public UpdateResellerUseCaseTests()
        {
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _loggerMock = new Mock<ILogger<UpdateResellerUseCase>>();

            _useCase = new UpdateResellerUseCase(
                _resellerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComDadosValidos_DeveAtualizarComSucesso()
        {
            // Arrange
            var command = _fixture.Create<UpdateResellerCommand>();
            var existingReseller = _fixture.Create<ResellerEntity>();
            var updatedReseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(existingReseller);

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
                .ReturnsAsync((ResellerEntity)null!);

            _resellerRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ResellerEntity>()))
                .ReturnsAsync(updatedReseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().Be(updatedReseller.Id);

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
            _resellerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ResellerEntity>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendaNaoEncontrada_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<UpdateResellerCommand>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync((ResellerEntity)null);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("Revenda não encontrada");

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
            _resellerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ResellerEntity>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ComCnpjJaExistenteEmOutraRevenda_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<UpdateResellerCommand>();
            var existingReseller = _fixture.Create<ResellerEntity>();
            var otherReseller = _fixture.Create<ResellerEntity>();

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(existingReseller);

            _resellerRepositoryMock.Setup(x => x.GetByCnpjAsync(command.Cnpj))
                .ReturnsAsync(otherReseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeFalse();
            result.ErrorMessage.Should().Be("CNPJ já cadastrado em outro sistema");

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
            _resellerRepositoryMock.Verify(x => x.GetByCnpjAsync(command.Cnpj), Times.Once);
            _resellerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ResellerEntity>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_ComMesmoCnpj_DeveAtualizarComSucesso()
        {
            // Arrange
            var command = _fixture.Create<UpdateResellerCommand>();
            var existingReseller = _fixture.Create<ResellerEntity>();
            var updatedReseller = _fixture.Create<ResellerEntity>();

            existingReseller.Cnpj = command.Cnpj;

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(command.Id))
                .ReturnsAsync(existingReseller);

            _resellerRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<ResellerEntity>()))
                .ReturnsAsync(updatedReseller);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();

            _resellerRepositoryMock.Verify(x => x.GetByIdAsync(command.Id), Times.Once);
            _resellerRepositoryMock.Verify(x => x.GetByCnpjAsync(command.Cnpj), Times.Never);
            _resellerRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ResellerEntity>()), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<UpdateResellerCommand>();

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