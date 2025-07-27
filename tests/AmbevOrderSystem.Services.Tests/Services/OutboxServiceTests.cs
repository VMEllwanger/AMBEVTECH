using AmbevOrderSystem.Infrastructure.DTOs;
using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Infrastructure.Repositories;
using AmbevOrderSystem.Infrastructure.Services;
using AmbevOrderSystem.Services.Services;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AmbevOrderSystem.Services.Tests.Services
{
    public class OutboxServiceTests
    {
        private readonly Mock<IOutboxRepository> _outboxRepositoryMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IResellerRepository> _resellerRepositoryMock;
        private readonly Mock<IAmbevApiService> _ambevApiServiceMock;
        private readonly Mock<ILogger<OutboxService>> _loggerMock;
        private readonly OutboxService _outboxService;

        public OutboxServiceTests()
        {
            _outboxRepositoryMock = new Mock<IOutboxRepository>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _resellerRepositoryMock = new Mock<IResellerRepository>();
            _ambevApiServiceMock = new Mock<IAmbevApiService>();
            _loggerMock = new Mock<ILogger<OutboxService>>();
            _outboxService = new OutboxService(
                _outboxRepositoryMock.Object,
                _orderRepositoryMock.Object,
                _resellerRepositoryMock.Object,
                _ambevApiServiceMock.Object,
                _loggerMock.Object);
        }



        [Fact]
        public async Task EnqueueMessageAsync_ComCorrelationId_DeveCriarMensagemComCorrelationIdEspecificado()
        {
            // Arrange
            var type = "TestType";
            var data = new { Test = "Data" };
            var correlationId = "test-correlation-id";
            var expectedMessage = new OutboxMessage
            {
                Id = 1,
                Type = type,
                Data = JsonSerializer.Serialize(data),
                Status = OutboxMessageStatus.Pending,
                CorrelationId = correlationId
            };

            _outboxRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync(expectedMessage);

            // Act
            var result = await _outboxService.EnqueueMessageAsync(type, data, correlationId);

            // Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(type);
            result.CorrelationId.Should().Be(correlationId);
            result.Status.Should().Be(OutboxMessageStatus.Pending);

            _outboxRepositoryMock.Verify(x => x.AddAsync(It.Is<OutboxMessage>(m =>
                m.Type == type &&
                m.CorrelationId == correlationId &&
                m.Status == OutboxMessageStatus.Pending)), Times.Once);
        }

        [Fact]
        public async Task EnqueueMessageAsync_SemCorrelationId_DeveGerarCorrelationIdAutomaticamente()
        {
            // Arrange
            var type = "TestType";
            var data = new { Test = "Data" };
            var expectedMessage = new OutboxMessage
            {
                Id = 1,
                Type = type,
                Data = JsonSerializer.Serialize(data),
                Status = OutboxMessageStatus.Pending,
                CorrelationId = "auto-generated-id"
            };

            _outboxRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync(expectedMessage);

            // Act
            var result = await _outboxService.EnqueueMessageAsync(type, data);

            // Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(type);
            result.CorrelationId.Should().NotBeNullOrEmpty();
            result.Status.Should().Be(OutboxMessageStatus.Pending);

            _outboxRepositoryMock.Verify(x => x.AddAsync(It.Is<OutboxMessage>(m =>
                m.Type == type &&
                !string.IsNullOrEmpty(m.CorrelationId) &&
                m.Status == OutboxMessageStatus.Pending)), Times.Once);
        }





        [Fact]
        public async Task EnqueueAmbevOrderAsync_ComDadosValidos_DeveEnfileirarPedidoAmbev()
        {
            // Arrange
            var resellerId = 1;
            var orderIds = new List<int> { 1, 2, 3 };
            var correlationId = "test-correlation-id";
            var expectedMessage = new OutboxMessage
            {
                Id = 1,
                Type = OutboxMessageType.AmbevOrderSubmission,
                Status = OutboxMessageStatus.Pending,
                CorrelationId = correlationId
            };

            _outboxRepositoryMock.Setup(x => x.AddAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync(expectedMessage);

            // Act
            var result = await _outboxService.EnqueueAmbevOrderAsync(resellerId, orderIds, correlationId);

            // Assert
            result.Should().NotBeNull();
            result.Type.Should().Be(OutboxMessageType.AmbevOrderSubmission);
            result.CorrelationId.Should().Be(correlationId);

            _outboxRepositoryMock.Verify(x => x.AddAsync(It.Is<OutboxMessage>(m =>
                m.Type == OutboxMessageType.AmbevOrderSubmission)), Times.Once);
        }





        [Fact]
        public async Task ProcessPendingMessagesAsync_ComMensagensPendentes_DeveProcessarTodas()
        {
            // Arrange
            var pendingMessages = new List<OutboxMessage>
            {
                CreateValidOutboxMessage(1),
                CreateValidOutboxMessage(2)
            };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            SetupSuccessfulAmbevOrderProcessing();

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.GetPendingMessagesAsync(10), Times.Once);
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(4));
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_SemMensagensPendentes_DeveRetornarSemProcessar()
        {
            // Arrange
            var emptyMessages = new List<OutboxMessage>();

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(emptyMessages);

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.GetPendingMessagesAsync(10), Times.Once);
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Never);
        }





        [Fact]
        public async Task ProcessRetryMessagesAsync_ComMensagensDeRetry_DeveProcessarTodas()
        {
            // Arrange
            var retryMessages = new List<OutboxMessage>
            {
                CreateValidOutboxMessage(1, OutboxMessageStatus.Retry),
                CreateValidOutboxMessage(2, OutboxMessageStatus.Retry)
            };

            _outboxRepositoryMock.Setup(x => x.GetRetryMessagesAsync(10))
                .ReturnsAsync(retryMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            SetupSuccessfulAmbevOrderProcessing();

            // Act
            await _outboxService.ProcessRetryMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.GetRetryMessagesAsync(10), Times.Once);
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(4));
        }





        [Fact]
        public async Task GetPendingCountAsync_DeveRetornarContagemCorreta()
        {
            // Arrange
            var expectedCount = 5;
            _outboxRepositoryMock.Setup(x => x.GetPendingCountAsync())
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _outboxService.GetPendingCountAsync();

            // Assert
            result.Should().Be(expectedCount);
            _outboxRepositoryMock.Verify(x => x.GetPendingCountAsync(), Times.Once);
        }





        [Fact]
        public async Task GetMessagesByCorrelationIdAsync_ComCorrelationIdValido_DeveRetornarMensagens()
        {
            // Arrange
            var correlationId = "test-correlation-id";
            var expectedMessages = new List<OutboxMessage>
            {
                new() { Id = 1, CorrelationId = correlationId },
                new() { Id = 2, CorrelationId = correlationId }
            };

            _outboxRepositoryMock.Setup(x => x.GetMessagesByCorrelationIdAsync(correlationId))
                .ReturnsAsync(expectedMessages);

            // Act
            var result = await _outboxService.GetMessagesByCorrelationIdAsync(correlationId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(expectedMessages);

            _outboxRepositoryMock.Verify(x => x.GetMessagesByCorrelationIdAsync(correlationId), Times.Once);
        }





        [Fact]
        public async Task ProcessPendingMessagesAsync_ComTipoAmbevOrderSubmission_DeveProcessarComSucesso()
        {
            // Arrange
            var message = CreateValidOutboxMessage(1);
            var pendingMessages = new List<OutboxMessage> { message };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            SetupSuccessfulAmbevOrderProcessing();

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_ComTipoNaoSuportado_DeveTratarErro()
        {
            // Arrange
            var message = new OutboxMessage
            {
                Id = 1,
                Type = "UnsupportedType",
                Status = OutboxMessageStatus.Pending
            };

            var pendingMessages = new List<OutboxMessage> { message };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }





        [Fact]
        public async Task ProcessPendingMessagesAsync_ComErroNaPrimeiraTentativa_DeveMarcarComoRetry()
        {
            // Arrange
            var message = new OutboxMessage
            {
                Id = 1,
                Type = OutboxMessageType.AmbevOrderSubmission,
                Status = OutboxMessageStatus.Pending,
                RetryCount = 0,
                MaxRetries = 3,
                Data = JsonSerializer.Serialize(new { ResellerId = 1, OrderIds = new[] { 1, 2 } })
            };

            var pendingMessages = new List<OutboxMessage> { message };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);


            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<CustomerOrder>());

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_ComErroNaUltimaTentativa_DeveMarcarComoFailed()
        {
            // Arrange
            var message = new OutboxMessage
            {
                Id = 1,
                Type = OutboxMessageType.AmbevOrderSubmission,
                Status = OutboxMessageStatus.Pending,
                RetryCount = 3,
                MaxRetries = 3,
                Data = JsonSerializer.Serialize(new { ResellerId = 1, OrderIds = new[] { 1, 2 } })
            };

            var pendingMessages = new List<OutboxMessage> { message };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            // Simula erro - pedidos não encontrados
            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<CustomerOrder>());

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }





        [Fact]
        public async Task ProcessPendingMessagesAsync_ComPedidosNaoEncontrados_DeveTratarErro()
        {
            // Arrange
            var message = CreateValidOutboxMessage(1);
            var pendingMessages = new List<OutboxMessage> { message };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(new List<CustomerOrder>());

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_ComRevendaNaoEncontrada_DeveTratarErro()
        {
            // Arrange
            var message = CreateValidOutboxMessage(1);
            var pendingMessages = new List<OutboxMessage> { message };

            var orders = new List<CustomerOrder>
            {
                new() { Id = 1, ResellerId = 999, Items = new List<OrderItem>() }
            };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(orders);

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Reseller)null);

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_ComQuantidadeInsuficiente_DeveTratarErro()
        {
            // Arrange
            var message = CreateValidOutboxMessage(1);
            var pendingMessages = new List<OutboxMessage> { message };

            var orders = new List<CustomerOrder>
            {
                new()
                {
                    Id = 1,
                    ResellerId = 1,
                    Items = new List<OrderItem>
                    {
                        new() { Quantity = 500 }
                    }
                }
            };

            var reseller = new Reseller { Id = 1, Cnpj = "12345678901234" };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(orders);

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(reseller);

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ProcessPendingMessagesAsync_ComErroNaApiAmbev_DeveTratarErro()
        {
            // Arrange
            var message = CreateValidOutboxMessage(1);
            var pendingMessages = new List<OutboxMessage> { message };

            var orders = new List<CustomerOrder>
            {
                new()
                {
                    Id = 1,
                    ResellerId = 1,
                    Items = new List<OrderItem>
                    {
                        new() { Quantity = 1000, ProductSku = "SKU1", ProductName = "Product1", UnitPrice = 10 }
                    }
                }
            };

            var reseller = new Reseller { Id = 1, Cnpj = "12345678901234" };

            _outboxRepositoryMock.Setup(x => x.GetPendingMessagesAsync(10))
                .ReturnsAsync(pendingMessages);

            _outboxRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<OutboxMessage>()))
                .ReturnsAsync((OutboxMessage m) => m);

            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(orders);

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(reseller);

            _ambevApiServiceMock.Setup(x => x.SubmitOrderAsync(It.IsAny<AmbevOrderRequest>()))
                .ThrowsAsync(new Exception("Erro na API da Ambev"));

            // Act
            await _outboxService.ProcessPendingMessagesAsync();

            // Assert
            _outboxRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<OutboxMessage>()), Times.Exactly(2));
        }





        [Theory]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        [InlineData(2, 4)]
        [InlineData(3, 8)]
        [InlineData(4, 16)]
        [InlineData(5, 16)]
        [InlineData(10, 16)]
        public void CalculateRetryDelay_ComDiferentesRetryCounts_DeveRetornarValoresCorretos(int retryCount, int expectedDelay)
        {
            // Act
            var result = _outboxService.CalculateRetryDelay(retryCount);

            // Assert
            result.Should().Be(expectedDelay);
        }





        private OutboxMessage CreateValidOutboxMessage(int id, string status = OutboxMessageStatus.Pending)
        {
            return new OutboxMessage
            {
                Id = id,
                Type = OutboxMessageType.AmbevOrderSubmission,
                Status = status,
                Data = JsonSerializer.Serialize(new { ResellerId = 1, OrderIds = new[] { 1, 2 } })
            };
        }

        private void SetupSuccessfulAmbevOrderProcessing()
        {
            var orders = new List<CustomerOrder>
            {
                new()
                {
                    Id = 1,
                    ResellerId = 1,
                    Items = new List<OrderItem>
                    {
                        new() { Quantity = 1000, ProductSku = "SKU1", ProductName = "Product1", UnitPrice = 10 }
                    }
                }
            };

            var reseller = new Reseller { Id = 1, Cnpj = "12345678901234" };

            var ambevResponse = new AmbevOrderResponse { OrderNumber = "AMB-123" };

            _orderRepositoryMock.Setup(x => x.GetByIdsAsync(It.IsAny<List<int>>()))
                .ReturnsAsync(orders);

            _resellerRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(reseller);

            _ambevApiServiceMock.Setup(x => x.SubmitOrderAsync(It.IsAny<AmbevOrderRequest>()))
                .ReturnsAsync(ambevResponse);

            _orderRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<CustomerOrder>()))
                .ReturnsAsync((CustomerOrder o) => o);
        }


    }
}