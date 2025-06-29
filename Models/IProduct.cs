﻿namespace Projekt.Models
{
    public interface IProduct
    {
        string Name { get; set; }

        string Description { get; set; }

        decimal Price { get; set; }

        List<Colors> Colors { get; set; }

        List<string> Sizes { get; set; }
        
        string? Image { get; set; }
        
        int BrandId { get; set; }
        
        int CategoryId { get; set; }
        
        int UserId { get; set; }

        List<Review> Reviews { get; set; }

        string BrandName { get; set; }

        string CategoryName { get; set; }

        string? UserName { get; set; }
    }
}