namespace Projekt.Models
{
    public class Order
    {
        public string Id { get; set; }

        public List<Product> Products { get; set; } = new List<Product>();

        public DateTime OrderedAt { get; set; } = DateTime.Now;
    }
}
