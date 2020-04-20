using SimpleMessageQueue;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.Models
{
    public class TitleUpdated : CommandBase, IMessage<TitleUpdated>
    {
        public readonly string ProductId;
        public readonly string Title;

        public TitleUpdated(long seq, string productId, string title)
            : base(seq)
        {
            ProductId = productId;
            Title = title;
        }

        public static TitleUpdated Create(Event @event, string productId, string title)
        {
            ThrowOnInvalidEvent(@event);
            ThrowOnInvalidProductId(productId);

            return new TitleUpdated(@event.Seq.Value, productId, title);
        }

        public TitleUpdated Clone()
        {
            return new TitleUpdated(Seq, ProductId, Title);
        }
    }
}