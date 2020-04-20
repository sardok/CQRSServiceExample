using Shared.Models;
using SimpleMessageQueue;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.Models
{
    public class ProductCreated : CommandBase, IMessage<ProductCreated>
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

        public ProductCreated Clone()
        {
            return new ProductCreated(Seq, Product.Clone());
        }
    }
}