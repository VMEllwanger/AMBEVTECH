using AmbevOrderSystem.Infrastructure.Entities;
using AmbevOrderSystem.Services.DTOs;
using AmbevOrderSystem.Services.Factories;

namespace AmbevOrderSystem.Services.Tests.Factories
{
    public class ResellerFactoryTests : BaseTest
    {
        private readonly ResellerFactory _factory;

        public ResellerFactoryTests()
        {
            _factory = new ResellerFactory();
        }

        [Fact]
        public async Task CreateAsync_ComDadosValidos_DeveCriarRevendaCorretamente()
        {
            // Arrange
            var request = _fixture.Build<CreateResellerRequest>().With(r => r.Cnpj, "12345678000199").Create();

            // Act
            var result = await _factory.CreateAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.RazaoSocial.Should().Be(request.RazaoSocial);
            result.NomeFantasia.Should().Be(request.NomeFantasia);
            result.Cnpj.Should().Be("12345678000199");
            result.Email.Should().Be(request.Email);
            result.Phones.Should().HaveCount(request.Phones.Count);
            result.Contacts.Should().HaveCount(request.Contacts.Count);
            result.DeliveryAddresses.Should().HaveCount(request.DeliveryAddresses.Count);
        }

        [Fact]
        public async Task CreateAsync_ComCnpjComPontuacao_DeveLimparPontuacao()
        {
            // Arrange 
            var request = _fixture.Build<CreateResellerRequest>().With(r => r.Cnpj, "12.345.678/0001-99").Create();
            request.Cnpj = "12.345.678/0001-99";

            // Act
            var result = await _factory.CreateAsync(request);

            // Assert
            result.Cnpj.Should().Be("12345678000199");
        }

        [Fact]
        public async Task CreateAsync_ComTelefonesValidos_DeveMapearTelefonesCorretamente()
        {
            // Arrange
            var phones1 = _fixture.Build<PhoneDto>()
                .With(p => p.IsPrimary, true).Create();
            var phones2 = _fixture.Build<PhoneDto>().With(p => p.IsPrimary, false).Create();

            var request = _fixture.Build<CreateResellerRequest>()
                .With(r => r.Phones, new List<PhoneDto> { phones1, phones2 })
                .Create();

            // Act
            var result = await _factory.CreateAsync(request);

            // Assert
            result.Phones.Should().HaveCount(2);
            result.Phones.Should().AllSatisfy(phone => phone.Should().NotBeNull());

            var primaryPhone = result.Phones.First(p => p.IsPrimary);
            primaryPhone.Number.Should().Be(request.Phones.First(p => p.IsPrimary).Number);
        }

        [Fact]
        public async Task CreateAsync_ComContatosValidos_DeveMapearContatosCorretamente()
        {
            // Arrange 
            var contact1 = _fixture.Build<ContactDto>()
                .With(c => c.IsPrimary, true).Create();
            var contact2 = _fixture.Build<ContactDto>().With(c => c.IsPrimary, false).Create();
            var request = _fixture.Build<CreateResellerRequest>()
                .With(r => r.Contacts, new List<ContactDto> { contact1, contact2 })
                .Create();

            // Act
            var result = await _factory.CreateAsync(request);

            // Assert
            result.Contacts.Should().HaveCount(2);
            result.Contacts.Should().AllSatisfy(contact => contact.Should().NotBeNull());

            var primaryContact = result.Contacts.First(c => c.IsPrimary);
            primaryContact.Name.Should().Be(request.Contacts.First(c => c.IsPrimary).Name);
        }

        [Fact]
        public async Task CreateAsync_ComEnderecosValidos_DeveMapearEnderecosCorretamente()
        {
            // Arrange 
            var address1 = _fixture.Build<DeliveryAddressDto>()
                .With(a => a.IsPrimary, true)
                .Create();
            var address2 = _fixture.Build<DeliveryAddressDto>().With(a => a.IsPrimary, false).Create();
            var request = _fixture.Build<CreateResellerRequest>()
                .With(r => r.DeliveryAddresses, new List<DeliveryAddressDto> { address1, address2 })
                .Create();

            // Act
            var result = await _factory.CreateAsync(request);

            // Assert
            result.DeliveryAddresses.Should().HaveCount(2);
            result.DeliveryAddresses.Should().AllSatisfy(address => address.Should().NotBeNull());

            var primaryAddress = result.DeliveryAddresses.First(a => a.IsPrimary);
            primaryAddress.Street.Should().Be(request.DeliveryAddresses.First(a => a.IsPrimary).Street);
        }

        [Fact]
        public async Task UpdateAsync_ComDadosValidos_DeveAtualizarRevendaCorretamente()
        {
            // Arrange 
            var existingReseller = _fixture.Create<Reseller>();
            var request = _fixture.Create<UpdateResellerRequest>();

            // Act
            var result = await _factory.UpdateAsync(existingReseller, request);

            // Assert
            result.Should().NotBeNull();
            result.RazaoSocial.Should().Be(request.RazaoSocial);
            result.NomeFantasia.Should().Be(request.NomeFantasia);
            result.Email.Should().Be(request.Email);
            result.Phones.Should().HaveCount(request.Phones.Count);
            result.Contacts.Should().HaveCount(request.Contacts.Count);
            result.DeliveryAddresses.Should().HaveCount(request.DeliveryAddresses.Count);
        }

        [Fact]
        public async Task UpdateAsync_DeveLimparListasExistentes()
        {
            // Arrange
            var existingReseller = _fixture.Create<Reseller>();
            var request = _fixture.Create<UpdateResellerRequest>();

            // Act
            var result = await _factory.UpdateAsync(existingReseller, request);

            // Assert
            result.Phones.Should().AllSatisfy(phone => phone.ResellerId.Should().Be(existingReseller.Id));
            result.Contacts.Should().AllSatisfy(contact => contact.ResellerId.Should().Be(existingReseller.Id));
            result.DeliveryAddresses.Should().AllSatisfy(address => address.ResellerId.Should().Be(existingReseller.Id));
        }

        [Fact]
        public async Task UpdateAsync_DeveAtualizarUpdatedAt()
        {
            // Arrange
            var existingReseller = _fixture.Create<Reseller>();
            // Define uma data no passado para garantir que será atualizada
            existingReseller.UpdatedAt = DateTime.UtcNow.AddHours(-1);
            var originalUpdatedAt = existingReseller.UpdatedAt;
            var request = _fixture.Create<UpdateResellerRequest>();

            // Act
            var result = await _factory.UpdateAsync(existingReseller, request);

            // Assert
            result.UpdatedAt.Should().BeAfter(originalUpdatedAt);
        }

    }
}