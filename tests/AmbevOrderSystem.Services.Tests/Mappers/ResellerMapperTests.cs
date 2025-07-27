using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Mappers;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AutoFixture;
using FluentAssertions;

namespace AmbevOrderSystem.Services.Tests.Mappers
{
    public class ResellerMapperTests : BaseTest
    {
        [Fact]
        public void ToEntity_ComCommandValido_DeveMapearCorretamente()
        {
            // Arrange
            var command = _fixture.Create<CreateResellerCommand>();

            // Act
            var result = ResellerMapper.ToEntity(command);

            // Assert
            result.Should().NotBeNull();
            result.RazaoSocial.Should().Be(command.RazaoSocial);
            result.NomeFantasia.Should().Be(command.NomeFantasia);
            result.Cnpj.Should().Be(command.Cnpj.Replace(".", "").Replace("/", "").Replace("-", ""));
            result.Email.Should().Be(command.Email);
            result.Phones.Should().HaveCount(command.Phones.Count);
            result.Contacts.Should().HaveCount(command.Contacts.Count);
            result.DeliveryAddresses.Should().HaveCount(command.DeliveryAddresses.Count);
        }

        [Fact]
        public void ToCreateResponse_ComEntidadeValida_DeveMapearCorretamente()
        {
            // Arrange
            var entity = _fixture.Create<Reseller>();

            // Act
            var result = ResellerMapper.ToCreateResponse(entity);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.RazaoSocial.Should().Be(entity.RazaoSocial);
            result.NomeFantasia.Should().Be(entity.NomeFantasia);
            result.Cnpj.Should().Be(entity.Cnpj);
            result.Email.Should().Be(entity.Email);
            result.Phones.Should().HaveCount(entity.Phones.Count);
            result.Contacts.Should().HaveCount(entity.Contacts.Count);
            result.DeliveryAddresses.Should().HaveCount(entity.DeliveryAddresses.Count);
            result.IsActive.Should().Be(entity.IsActive);
            result.CreatedDate.Should().Be(entity.CreatedAt);
        }

        [Fact]
        public void UpdateEntity_ComCommandValido_DeveAtualizarCorretamente()
        {
            // Arrange
            var entity = _fixture.Create<Reseller>();
            var command = _fixture.Create<UpdateResellerCommand>();

            // Act
            ResellerMapper.UpdateEntity(entity, command);

            // Assert
            entity.RazaoSocial.Should().Be(command.RazaoSocial);
            entity.NomeFantasia.Should().Be(command.NomeFantasia);
            entity.Cnpj.Should().Be(command.Cnpj);
            entity.Email.Should().Be(command.Email);
            entity.Phones.Should().HaveCount(command.Phones.Count);
            entity.Contacts.Should().HaveCount(command.Contacts.Count);
            entity.DeliveryAddresses.Should().HaveCount(command.DeliveryAddresses.Count);
        }

        [Fact]
        public void MapToResellerDto_ComEntidadeValida_DeveMapearCorretamente()
        {
            // Arrange
            var entity = _fixture.Create<Reseller>();

            // Act
            var result = ResellerMapper.MapToResellerDto(entity);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(entity.Id);
            result.Cnpj.Should().Be(entity.Cnpj);
            result.RazaoSocial.Should().Be(entity.RazaoSocial);
            result.NomeFantasia.Should().Be(entity.NomeFantasia);
            result.Email.Should().Be(entity.Email);
            result.Phones.Should().HaveCount(entity.Phones.Count);
            result.Contacts.Should().HaveCount(entity.Contacts.Count);
            result.DeliveryAddresses.Should().HaveCount(entity.DeliveryAddresses.Count);
            result.CreatedAt.Should().Be(entity.CreatedAt);
            result.UpdatedAt.Should().Be(entity.UpdatedAt);
        }

        [Fact]
        public void ToGetResponseList_ComListaValida_DeveMapearCorretamente()
        {
            // Arrange
            var entities = _fixture.CreateMany<Reseller>(3).ToList();

            // Act
            var result = ResellerMapper.ToGetResponseList(entities);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().AllSatisfy(item => item.Should().NotBeNull());
        }

        [Fact]
        public void ToGetResponseList_ComListaVazia_DeveRetornarListaVazia()
        {
            // Arrange
            var entities = new List<Reseller>();

            // Act
            var result = ResellerMapper.ToGetResponseList(entities);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ToEntity_ComCnpjComPontuacao_DeveLimparPontuacao()
        {
            // Arrange
            var command = _fixture.Build<CreateResellerCommand>().With(c => c.Cnpj, "12.345.678/0001-99").Create();

            // Act
            var result = ResellerMapper.ToEntity(command);

            // Assert
            result.Cnpj.Should().Be("12345678000199");
        }

        [Fact]
        public void UpdateEntity_ComListasNulas_DeveCriarListasVazias()
        {
            // Arrange
            var entity = _fixture.Create<Reseller>();
            var command = _fixture.Create<UpdateResellerCommand>();
            command.Phones = null!;
            command.Contacts = null!;
            command.DeliveryAddresses = null!;

            // Act
            ResellerMapper.UpdateEntity(entity, command);

            // Assert
            entity.Phones.Should().NotBeNull();
            entity.Phones.Should().BeEmpty();
            entity.Contacts.Should().NotBeNull();
            entity.Contacts.Should().BeEmpty();
            entity.DeliveryAddresses.Should().NotBeNull();
            entity.DeliveryAddresses.Should().BeEmpty();
        }
    }
}