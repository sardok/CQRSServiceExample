using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.Models
{
    public class AmountUpdated : CommandBase
    {
        public readonly string ProductId;
        public readonly int Amount;

        public AmountUpdated(long seq, string productId, int amount)
            : base(seq)
        {
            ProductId = productId;
            Amount = amount;
        }

        public static AmountUpdated Create(Event @event, string productId, int amount)
        {
            ThrowOnInvalidEvent(@event);
            ThrowOnInvalidProductId(productId);

            return new AmountUpdated(@event.Seq.Value, productId, amount);
        }
    }
}