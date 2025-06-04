using System.Collections.Generic;
using Projekt.Models;

namespace Projekt.Services
{
  
    public class OrderService
    {
        private readonly List<Products> _orderedProducts = new();

     
        public void AddProduct(Products p)
        {
            if (p is null) return;
            _orderedProducts.Add(p);
        }


        public void RemoveProduct(Products p)
        {
            if (p is null) return;
            _orderedProducts.Remove(p);
        }

       
        public void Clear()
        {
            _orderedProducts.Clear();
        }

     
        public IReadOnlyList<Products> GetAllOrderedProducts() =>
            _orderedProducts.AsReadOnly();
    }
}
