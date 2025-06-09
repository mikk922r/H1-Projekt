using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class ReviewFormModel : IReview
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }

        [Required(ErrorMessage = "Titel er påkrævet")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Beskrivelse er påkrævet")]
        [MinLength(10, ErrorMessage = "Beskrivelsen skal være mindst 10 tegn langt")]
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bedømmelse er påkrævet")]
        [Range(1, 5, ErrorMessage = "Bedømmelse skal være mellem 1 og 5")]
        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserName { get; set; } = string.Empty;
    }
}
