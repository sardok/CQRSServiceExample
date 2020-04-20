using System;
using ProductServiceCQRSLib.Models.Command;

namespace ProductServiceCQRSLib.Models
{
    public abstract class CommandBase
    {
        public readonly long Seq;

        public CommandBase(long seq)
        {
            Seq = seq;
        }

        public static void ThrowOnInvalidEvent(Event @event)
        {
            if (!@event.Seq.HasValue)
                throw new ArgumentException(nameof(@event.Seq));
        }
        
        public static void ThrowOnInvalidProductId(string productId)
        {
            if (string.IsNullOrEmpty(productId))
                throw new ArgumentException(nameof(productId));
        }
    }
}
