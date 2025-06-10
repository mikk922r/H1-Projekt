using Projekt.Models;

namespace Projekt.Services
{
    public class CartService
    {
        public event EventHandler<List<Product>> OnCartHasChanged;

        private readonly List<Product> _cart = new List<Product>();

        public void AddToCart(Product product)
        {
            _cart.Add(product);

            OnCartChanged(_cart);
        }

        public void AddToCart(List<Product> products)
        {
            _cart.AddRange(products);

            OnCartChanged(_cart);
        }

        public void RemoveFromCart(Product item)
        {
            _cart.Remove(item);

            OnCartChanged(_cart);
        }

        public void RemoveFromCart(List<Product> items)
        {
            foreach (Product item in items)
            {
                _cart.Remove(item);
            }

            OnCartChanged(_cart);
        }

        public IReadOnlyList<Product> GetCart() => _cart.AsReadOnly();

        public void ClearCart()
        {
            _cart.Clear();

            OnCartChanged(_cart);
        }

        protected virtual void OnCartChanged(List<Product> e)
        {
            EventHandler<List<Product>> handler = OnCartHasChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
