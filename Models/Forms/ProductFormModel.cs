using System.ComponentModel.DataAnnotations;

namespace Projekt.Models.Forms
{
    public class ProductFormModel : IProduct
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Navn er påkrævet")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Beskrivelse er påkrævet")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pris er påkrævet")]
        [Range(1, 100000, ErrorMessage = "Pris skal være mellem 1 og 999.999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Farve er påkrævet")]
        [MinLength(1, ErrorMessage = "Mindst én farve skal vælges")]
        public List<Colors> Colors { get; set; } = new List<Colors>();

        [Required(ErrorMessage = "Størrelse er påkrævet")]
        [MinLength(1, ErrorMessage = "Mindst én størrelse skal vælges")]
        public List<string> Sizes { get; set; } = new List<string>();

        public string? Image { get; set; }

        [Required(ErrorMessage = "Mærke er påkrævet")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Kategori er påkrævet")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Bruger er påkrævet")]
        public int UserId { get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();

        public string BrandName { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;

        public string? UserName { get; set; }

        public Product ToEntity()
        {
            return new Product
            {
                Id = Id ?? 0,
                Name = Name,
                Description = Description,
                Price = Price,
                Colors = Colors,
                Sizes = Sizes,
                Image = Image,
                BrandId = BrandId,
                CategoryId = CategoryId,
                UserId = UserId
            };
        }
    }
}
