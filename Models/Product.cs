using System.ComponentModel.DataAnnotations;

namespace Projekt.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Price { get; set; }

        public string? Color { get; set; }

        public string? Size { get; set; }
        public int Quantity { get; set; }

        public bool Used { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public int UserId { get; set; }

        public string? Image { get; set; }

        public string BrandName { get; set; } = default!;
        public string CategoryName { get; set; } = default!;
        public string? UserName { get; set; }

    }
}