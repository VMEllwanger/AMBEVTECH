using AmbevOrderSystem.Services.Implementations;

namespace AmbevOrderSystem.Services.Tests.Validations
{
    public class ValidationServiceTests
    {
        private readonly ValidationService _validationService;

        public ValidationServiceTests()
        {
            _validationService = new ValidationService();
        }

        [Theory]
        [InlineData("11222333000181", true)]  // CNPJ válido
        [InlineData("11.222.333/0001-81", true)]  // CNPJ válido com formatação
        [InlineData("12345678000190", false)]  // CNPJ inválido
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("123456789012345", false)]
        [InlineData("11111111111111", false)]
        public void IsValidCnpj_ComDiferentesValores_DeveRetornarResultadoCorreto(string cnpj, bool expected)
        {
            // Act
            var result = _validationService.IsValidCnpj(cnpj);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("teste@email.com", true)]
        [InlineData("teste@email.co.uk", true)]
        [InlineData("teste+tag@email.com", true)]
        [InlineData("teste.email@domain.com", true)]
        [InlineData("", false)]
        [InlineData("invalid-email", false)]
        [InlineData("@email.com", false)]
        [InlineData("teste@", false)]
        [InlineData("teste@.com", false)]
        public void IsValidEmail_ComDiferentesValores_DeveRetornarResultadoCorreto(string email, bool expected)
        {
            // Act
            var result = _validationService.IsValidEmail(email);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("11999999999", true)]  // Celular com DDD 11
        [InlineData("1199999999", true)]   // Telefone fixo com DDD 11
        [InlineData("5511999999999", true)] // Celular com código do país
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("119999999999", false)]  // Muito longo
        [InlineData("119999999", false)]     // Muito curto
        [InlineData("11888888888", false)]   // Celular que não começca com 9
        public void IsValidPhone_ComDiferentesValores_DeveRetornarResultadoCorreto(string phone, bool expected)
        {
            // Act
            var result = _validationService.IsValidPhone(phone);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("11144477735", true)]   // CPF válido
        [InlineData("111.444.777-35", true)] // CPF válido com formatação
        [InlineData("12345678900", false)]  // CPF inválido
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("123456789012", false)]
        [InlineData("11111111111", false)]
        public void IsValidCpf_ComDiferentesValores_DeveRetornarResultadoCorreto(string cpf, bool expected)
        {
            // Act
            var result = _validationService.IsValidCpf(cpf);

            // Assert
            result.Should().Be(expected);
        }

        [Theory]
        [InlineData("01234-567", true)]
        [InlineData("01234567", true)]
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("01234-56", false)]
        [InlineData("0123456", false)]
        [InlineData("012345678", false)]
        public void IsValidCep_ComDiferentesValores_DeveRetornarResultadoCorreto(string cep, bool expected)
        {
            // Act
            var result = _validationService.IsValidCep(cep);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidCnpj_ComCnpjValidoComPontuacao_DeveRetornarTrue()
        {
            // Arrange
            var cnpj = "11.222.333/0001-81";

            // Act
            var result = _validationService.IsValidCnpj(cnpj);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidCpf_ComCpfValidoComPontuacao_DeveRetornarTrue()
        {
            // Arrange
            var cpf = "111.444.777-35";

            // Act
            var result = _validationService.IsValidCpf(cpf);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidPhone_ComTelefoneComDDD55_DeveRetornarTrue()
        {
            // Arrange
            var phone = "5511999999999";

            // Act
            var result = _validationService.IsValidPhone(phone);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidCep_ComCepSemHifen_DeveRetornarTrue()
        {
            // Arrange
            var cep = "01234567";

            // Act
            var result = _validationService.IsValidCep(cep);

            // Assert
            result.Should().BeTrue();
        }
    }
}