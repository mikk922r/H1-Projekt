// Services/OrderService.cs
using System.Collections.Generic;
using Projekt.Models;   // <-- import your Product model namespace

namespace Projekt.Services
{
    /// <summary>
    /// A scoped service that keeps an in‐memory list of Product for the current session.
    /// </summary>
    public class OrderService
    {
        private readonly List<Product> _orderedProducts = new();

        /// <summary>
        /// Returns a read‐only snapshot of the current ordered products.
        /// </summary>
        public IReadOnlyList<Product> GetAllOrderedProducts()
            => _orderedProducts.AsReadOnly();

        /// <summary>
        /// Add a range of products to the orders list.
        /// </summary>
        public void AddProducts(List<Product> products)
        {
            if (products is null)
            {
                return;
            }

            _orderedProducts.AddRange(products);
        }

        /// <summary>
        /// Remove a product (matching by Id).
        /// </summary>
        public void RemoveProduct(Product product)
        {
            if (product == null) return;
            _orderedProducts.RemoveAll(p => p.Id == product.Id);
        }

        /// <summary>
        /// Clear the entire orders list.
        /// </summary>
        public void ClearOrders()
        {
            _orderedProducts.Clear();
        }
    }
}
