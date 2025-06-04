using System.Collections.Generic;
using Projekt.Models;

namespace Projekt.Services
{
  
    public class OrderService
    {
        private readonly List<Product> _orderedProducts = new();

     
        public void AddProduct(Product p)
        {
            if (p is null) return;
            _orderedProducts.Add(p);
        }


        public void RemoveProduct(Product p)
        {
            if (p is null) return;
            _orderedProducts.Remove(p);
        }

       
        public void Clear()
        {
            _orderedProducts.Clear();
        }

     
        public IReadOnlyList<Product> GetAllOrderedProducts() =>
            _orderedProducts.AsReadOnly();
    }
}
