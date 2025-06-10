using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class RegisterFormModel : UserFormModel, IUser
    {
        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Adgangskode skal være mindst 8 tegn langt")]
        public string Password { get; set; } = string.Empty;
    }
}
