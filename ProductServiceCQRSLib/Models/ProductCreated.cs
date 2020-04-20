using Shared.Models;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.Models
{
    public class ProductCreated : CommandBase
    {
        public readonly Product Product;

        public ProductCreated(long seq, Product product)
            : base(seq)
        {
            Product = product;
        }

        public static ProductCreated Create(Event @event, Product product)
        {
            ThrowOnInvalidEvent(@event);
            ThrowOnInvalidProductId(product?.Id);

            return new ProductCreated(@event.Seq.Value, product);
        }
    }
}