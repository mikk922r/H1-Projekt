﻿using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class LoginFormModel
    {
        [Required(ErrorMessage = "Email er påkrævet")]
        [EmailAddress(ErrorMessage = "Email skal være gyldig")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adgangskode er påkrævet")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Adgangskode skal være mindst 8 tegn langt")]
        public string Password { get; set; } = string.Empty;
    }
}
