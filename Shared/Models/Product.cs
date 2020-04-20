namespace Shared.Models
{
    public class Product
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Amount { get; set; }

        public Product Clone()
        {
            return new Product
            {
                Id = Id,
                Title = Title,
                Amount = Amount,
            };
        }
    }
}