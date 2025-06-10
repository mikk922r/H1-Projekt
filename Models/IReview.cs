namespace Projekt.Models
{
    public interface IReview
    {
        int ProductId { get; set; }

        int UserId { get; set; }

        string Title { get; set; }

        string Message { get; set; }

        int Rating { get; set; }

        DateTime CreatedAt { get; set; }

        string UserName { get; set; }
    }
}
