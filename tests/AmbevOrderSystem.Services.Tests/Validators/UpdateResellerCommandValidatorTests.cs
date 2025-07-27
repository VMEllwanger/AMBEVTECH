using AmbevOrderSystem.Services.Interfaces;
using AmbevOrderSystem.Services.Models.Commands.Reseller;
using AmbevOrderSystem.Services.Validators;
using FluentValidation.TestHelper;

namespace AmbevOrderSystem.Services.Tests.Validators
{
    public class UpdateResellerCommandValidatorTests
    {
        private readonly Mock<IValidationService> _validationServiceMock;
        private readonly UpdateResellerCommandValidator _validator;
        private readonly Fixture _fixture;

        public UpdateResellerCommandValidatorTests()
        {
            _validationServiceMock = new Mock<IValidationService>();
            _validator = new UpdateResellerCommandValidator(_validationServiceMock.Object);
            _fixture = new Fixture();
        }

        [Fact]
        public void Validate_ComDadosValidos_DevePassar()
        {
            // Arrange
            var command = CreateValidCommand();

            _validationServiceMock.Setup(x => x.IsValidCnpj(It.IsAny<string>())).Returns(true);
            _validationServiceMock.Setup(x => x.IsValidEmail(It.IsAny<string>())).Returns(true);
            _validationServiceMock.Setup(x => x.IsValidPhone(It.IsAny<string>())).Returns(true);
            _validationServiceMock.Setup(x => x.IsValidCep(It.IsAny<string>())).Returns(true);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void Validate_ComIdInvalido_DeveFalhar(int id)
        {
            // Arrange
            var command = CreateValidCommand();
            command.Id = id;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Id);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ComRazaoSocialInvalida_DeveFalhar(string razaoSocial)
        {
            // Arrange
            var command = CreateValidCommand();
            command.RazaoSocial = razaoSocial;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RazaoSocial);
        }

        [Fact]
        public void Validate_ComRazaoSocialMuitoLonga_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.RazaoSocial = new string('A', 101);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.RazaoSocial);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ComNomeFantasiaInvalido_DeveFalhar(string nomeFantasia)
        {
            // Arrange
            var command = CreateValidCommand();
            command.NomeFantasia = nomeFantasia;

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.NomeFantasia);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ComCnpjInvalido_DeveFalhar(string cnpj)
        {
            // Arrange
            var command = CreateValidCommand();
            command.Cnpj = cnpj;

            _validationServiceMock.Setup(x => x.IsValidCnpj(cnpj)).Returns(false);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Cnpj);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ComEmailInvalido_DeveFalhar(string email)
        {
            // Arrange
            var command = CreateValidCommand();
            command.Email = email;

            _validationServiceMock.Setup(x => x.IsValidEmail(email)).Returns(false);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_ComEmailMuitoLongo_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Email = new string('a', 151) + "@test.com";

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Validate_ComTelefoneInvalido_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Phones = new List<PhoneCommand> { new() { Number = "invalid" } };

            _validationServiceMock.Setup(x => x.IsValidPhone("invalid")).Returns(false);

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Phones);
        }

        [Fact]
        public void Validate_ComListaContatosVazia_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Contacts = new List<ContactCommand>();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contacts);
        }

        [Fact]
        public void Validate_ComContatoSemNome_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Contacts = new List<ContactCommand> { new() { Name = "", IsPrimary = true } };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contacts);
        }

        [Fact]
        public void Validate_ComContatoSemPrincipal_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.Contacts = new List<ContactCommand> { new() { Name = "Test", IsPrimary = false } };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Contacts);
        }

        [Fact]
        public void Validate_ComListaEnderecosVazia_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.DeliveryAddresses = new List<DeliveryAddressCommand>();

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DeliveryAddresses);
        }

        [Fact]
        public void Validate_ComEnderecoSemPrincipal_DeveFalhar()
        {
            // Arrange
            var command = CreateValidCommand();
            command.DeliveryAddresses = new List<DeliveryAddressCommand> { CreateInvalidAddress() };

            // Act
            var result = _validator.TestValidate(command);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DeliveryAddresses);
        }

        private UpdateResellerCommand CreateValidCommand()
        {
            return new UpdateResellerCommand
            {
                Id = 1,
                RazaoSocial = "Empresa Teste LTDA",
                NomeFantasia = "Empresa Teste",
                Cnpj = "12345678000199",
                Email = "teste@empresa.com",
                Phones = new List<PhoneCommand> { new() { Number = "11999999999", IsPrimary = true } },
                Contacts = new List<ContactCommand> { new() { Name = "João Silva", IsPrimary = true } },
                DeliveryAddresses = new List<DeliveryAddressCommand> { CreateValidAddress() }
            };
        }

        private DeliveryAddressCommand CreateValidAddress()
        {
            return new DeliveryAddressCommand
            {
                Street = "Rua Teste",
                Number = "123",
                Complement = "Apto 1",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                ZipCode = "01234-567",
                IsPrimary = true
            };
        }

        private DeliveryAddressCommand CreateInvalidAddress()
        {
            return new DeliveryAddressCommand
            {
                Street = "",
                Number = "",
                Neighborhood = "",
                City = "",
                State = "",
                ZipCode = "",
                IsPrimary = false
            };
        }
    }
}