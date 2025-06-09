using Projekt.Models;
using System.Globalization;

namespace Projekt.Helpers
{
    public static class ReviewsHelper
    {
        public static decimal GetAverageRating(List<Review> reviews)
        {
            if (reviews == null || reviews.Count == 0)
            {
                return 0;
            }

            decimal totalRating = 0;

            foreach (var review in reviews)
            {
                totalRating += review.Rating;
            }

            return Math.Round(totalRating / reviews.Count, 1, MidpointRounding.AwayFromZero);
        }

        public static int GetRoundRating(List<Review> reviews)
        {
            decimal rating = GetAverageRating(reviews);

            return (int)Math.Round(rating, MidpointRounding.AwayFromZero);
        }

        public static int GetTotalReviews(List<Review> reviews)
        {
            return reviews?.Count ?? 0;
        }

        public static string GetTimeAgo(DateTime dateTime)
        {
            TimeSpan timeSpan = DateTimeOffset.Now - dateTime.AddHours(2); // 2 timers forskel på databasen og os af en eller anden grund

            if (timeSpan.TotalDays > 30)
            {
                return dateTime.ToString("d. MMMM yyyy", CultureInfo.CurrentCulture);
            }

            if (timeSpan.TotalDays > 7)
            {
                int weeks = (int)(timeSpan.TotalDays / 7);

                return $"{weeks} {(weeks == 1 ? "uge" : "uger")} siden";
            }

            if (timeSpan.TotalDays >= 1)
            {
                int days = (int)timeSpan.TotalDays;

                return $"{days} {(days == 1 ? "dag" : "dage")} siden";
            }

            if (timeSpan.TotalHours >= 1)
            {
                int hours = (int)timeSpan.TotalHours;

                return $"{hours} {(hours == 1 ? "time" : "timer")} siden";
            }

            if (timeSpan.TotalMinutes >= 1)
            {
                int minutes = (int)timeSpan.TotalMinutes;

                return $"{minutes} {(minutes == 1 ? "minut" : "minutter")} siden";
            }

            return "Lige nu";
        }
    }
}
