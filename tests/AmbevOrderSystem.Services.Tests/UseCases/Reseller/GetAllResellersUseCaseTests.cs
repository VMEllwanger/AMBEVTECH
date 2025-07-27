using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.UseCases.Reseller;
using Microsoft.Extensions.Logging;
using ResellerEntity = AmbevOrderSystem.Infrastructure.Entities.Reseller;

namespace AmbevOrderSystem.Services.Tests.UseCases.Reseller
{
    public class GetAllResellersUseCaseTests : BaseTest
    {
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<ILogger<GetAllResellersUseCase>> _loggerMock;
        private readonly GetAllResellersUseCase _useCase;

        public GetAllResellersUseCaseTests()
        {
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _loggerMock = new Mock<ILogger<GetAllResellersUseCase>>();

            _useCase = new GetAllResellersUseCase(
                _resellerRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task ExecuteAsync_ComRevendasExistentes_DeveRetornarLista()
        {
            // Arrange
            var command = _fixture.Create<GetAllResellersCommand>();
            var resellers = _fixture.CreateMany<ResellerEntity>(3).ToList();

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(resellers);

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Resellers.Should().HaveCount(3);
            result.Value.TotalCount.Should().Be(3);

            _resellerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var command = _fixture.Create<GetAllResellersCommand>();

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<ResellerEntity>());

            // Act
            var result = await _useCase.ExecuteAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Resellers.Should().BeEmpty();
            result.Value.TotalCount.Should().Be(0);

            _resellerRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task ExecuteAsync_ComExcecao_DeveRetornarFalha()
        {
            // Arrange
            var command = _fixture.Create<GetAllResellersCommand>();

            _resellerRepositoryMock.Setup(x => x.GetAllAsync())
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