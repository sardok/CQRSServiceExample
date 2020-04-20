using SimpleMessageQueue;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.Models
{
    public class ProductDeleted : CommandBase, IMessage<ProductDeleted>
    {
        public readonly string ProductId;

        public ProductDeleted(long seq, string productId)
            : base(seq)
        {
            ProductId = productId;
        }

        public static ProductDeleted Create(Event @event, string productId)
        {
            ThrowOnInvalidEvent(@event);
            ThrowOnInvalidProductId(productId);

            return new ProductDeleted(@event.Seq.Value, productId);
        }

        public ProductDeleted Clone()
        {
            return new ProductDeleted(Seq, ProductId);
        }
    }
}
