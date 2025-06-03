using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class RegisterForm : IUser
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(2, ErrorMessage = "Name must be at least 2 characters long")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Must be a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone code is required")]
        [Range(1, 999, ErrorMessage = "Phone code must be between 1 and 999")]
        public int PhoneCode { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Must be a valid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; } = string.Empty;
    }
}
