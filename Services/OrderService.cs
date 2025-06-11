using Projekt.Models;

namespace Projekt.Services
{
    public class OrderService
    {
        private readonly List<Order> _orders = new();

        public void CreateOrder(List<Product> products)
        {
            if (products is null)
            {
                return;
            }

            string id = Random.Shared.Next(0, 1000000).ToString("D6");

            _orders.Add(new Order
            {
                Id = id,
                Products = products,
            });
        }

        public IReadOnlyList<Order> GetOrders() => _orders.AsReadOnly();
    }
}
