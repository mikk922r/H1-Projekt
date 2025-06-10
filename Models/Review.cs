namespace Projekt.Models
{
    public class Review : IReview
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        public int UserId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public int Rating { get; set; }

        public DateTime CreatedAt { get; set; }

        public string UserName { get; set; } = string.Empty;
    }
}
