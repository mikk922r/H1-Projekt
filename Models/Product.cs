using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Product : IProduct
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public List<Colors> Colors { get; set; } = new List<Colors>();

        public List<string> Sizes { get; set; } = new List<string>();

        public string? Image { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int UserId { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();

        public string BrandName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string? UserName { get; set; }
    }
}