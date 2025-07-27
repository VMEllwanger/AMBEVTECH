using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.Mappers;
using AutoFixture;
using FluentAssertions;

namespace AmbevOrderSystem.Services.Tests.Mappers
{
    public class OrderMapperTests : BaseTest
    {
        [Fact]
        public void ToCreateResponse_ComEntidadeValida_DeveMapearCorretamente()
        {
            // Arrange
            var entity = _fixture.Create<CustomerOrder>();

            // Act
            var result = OrderMapper.ToCreateResponse(entity);

            // Assert
            result.Should().NotBeNull();
            result.OrderId.Should().Be(entity.Id);
            result.CustomerIdentification.Should().Be(entity.CustomerIdentification);
            result.Status.Should().Be(entity.Status.ToString());
            result.OrderCreatedAt.Should().Be(entity.CreatedAt);
            result.Items.Should().HaveCount(entity.Items.Count);
        }

        [Fact]
        public void ToGetResponse_ComEntidadeValida_DeveMapearCorretamente()
        {
            // Arrange
            var entity = _fixture.Create<CustomerOrder>();

            // Act
            var result = OrderMapper.ToGetResponse(entity);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.ResellerId.Should().Be(entity.ResellerId);
            result.Status.Should().Be(entity.Status.ToString());
            result.AmbevOrderNumber.Should().Be(entity.AmbevOrderNumber);
            result.OrderCreatedAt.Should().Be(entity.CreatedAt);
            result.Items.Should().HaveCount(entity.Items.Count);
        }

        [Fact]
        public void ToGetResponseList_ComListaValida_DeveMapearCorretamente()
        {
            // Arrange
            var entities = _fixture.CreateMany<CustomerOrder>(3).ToList();

            // Act
            var result = OrderMapper.ToGetResponseList(entities);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().AllSatisfy(item => item.Should().NotBeNull());
        }

        [Fact]
        public void ToGetResponseList_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var entities = new List<CustomerOrder>();

            // Act
            var result = OrderMapper.ToGetResponseList(entities);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void MapToCustomerOrderDto_ComEntidadeValida_DeveMapearCorretamente()
        {
            // Arrange
            var entity = _fixture.Create<CustomerOrder>();

            // Act
            var result = OrderMapper.MapToCustomerOrderDto(entity);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.CustomerIdentification.Should().Be(entity.CustomerIdentification);
            result.ResellerId.Should().Be(entity.ResellerId);
            result.Status.Should().Be(entity.Status.ToString());
            result.CreatedAt.Should().Be(entity.CreatedAt);
            result.AmbevOrderNumber.Should().Be(entity.AmbevOrderNumber);
            result.Items.Should().HaveCount(entity.Items.Count);
        }

        [Fact]
        public void MapToCustomerOrderDto_ComResellerNulo_DeveMapearComNomeVazio()
        {
            // Arrange
            var entity = _fixture.Create<CustomerOrder>();
            entity.Reseller = null!;

            // Act
            var result = OrderMapper.MapToCustomerOrderDto(entity);

            // Assert
            result.Should().NotBeNull();
            result.ResellerName.Should().BeEmpty();
        }

        [Fact]
        public void MapToCustomerOrderDtoList_ComListaValida_DeveMapearCorretamente()
        {
            // Arrange
            var entities = _fixture.CreateMany<CustomerOrder>(3).ToList();

            // Act
            var result = OrderMapper.MapToCustomerOrderDtoList(entities);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().AllSatisfy(item => item.Should().NotBeNull());
        }

        [Fact]
        public void MapToCustomerOrderDtoList_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var entities = new List<CustomerOrder>();

            // Act
            var result = OrderMapper.MapToCustomerOrderDtoList(entities);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }
    }
}