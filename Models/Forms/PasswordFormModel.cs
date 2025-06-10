using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class PasswordFormModel
    {
        [Required(ErrorMessage = "Nuværende adgangskode er påkrævet")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Adgangskode skal være mindst 8 tegn langt")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bekræft adgangskode er påkrævet")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Bekræft adgangskode skal matche adgangskode")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
