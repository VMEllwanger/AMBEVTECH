namespace AmbevOrderSystem.Services.DTOs
{
    public class CreateResellerRequest
    {
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneDto> Phones { get; set; } = new();
        public List<ContactDto> Contacts { get; set; } = new();
        public List<DeliveryAddressDto> DeliveryAddresses { get; set; } = new();
    }

    public class UpdateResellerRequest
    {
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneDto> Phones { get; set; } = new();
        public List<ContactDto> Contacts { get; set; } = new();
        public List<DeliveryAddressDto> DeliveryAddresses { get; set; } = new();
    }

    public class ResellerDto
    {
        public int Id { get; set; }
        public string Cnpj { get; set; } = string.Empty;
        public string RazaoSocial { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<PhoneDto> Phones { get; set; } = new();
        public List<ContactDto> Contacts { get; set; } = new();
        public List<DeliveryAddressDto> DeliveryAddresses { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class PhoneDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }


    public class ContactDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }


    public class DeliveryAddressDto
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Complement { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
    }
}