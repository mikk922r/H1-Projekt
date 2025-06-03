using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class RegisterForm : IUser
    {
        [Required(ErrorMessage = "Navn er påkrævet")]
        [MinLength(2, ErrorMessage = "Navn skal være mindst 2 tegn langt")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Email skal være gyldig")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefonkode er påkrævet")]
        [Range(1, 999, ErrorMessage = "Telefonkode skal være mellem 1 og 999")]
        public int PhoneCode { get; set; }

        [Required(ErrorMessage = "Telefonnummer er påkrævet")]
        [Phone(ErrorMessage = "Telefonnummer skal være gyldigt")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Adgangskode skal være mindst 8 tegn langt")]
        public string Password { get; set; } = string.Empty;
    }
}
