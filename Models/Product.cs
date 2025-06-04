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

        public Colors Color { get; set; }

        public string Size { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public bool Used { get; set; }

        public string? Image { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int UserId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string? UserName { get; set; }
    }
}