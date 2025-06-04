namespace Projekt.Models
{
    public interface IProduct
    {
        string Name { get; set; }

        string Description { get; set; }

        decimal Price { get; set; }

        Colors Color { get; set; }

        string Size { get; set; }
        
        int Quantity { get; set; }
        
        bool Used { get; set; }

        string? Image { get; set; }
        
        int BrandId { get; set; }
        
        int CategoryId { get; set; }
        
        int UserId { get; set; }

        string BrandName { get; set; }

        string CategoryName { get; set; }

        string? UserName { get; set; }
    }
}