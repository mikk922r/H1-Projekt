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

        public void RemoveFromCart(Product item)
        {
            _cart.Remove(item);

            OnCartChanged(_cart);
        }

        public IReadOnlyList<Product> GetCart() => _cart.AsReadOnly();

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
