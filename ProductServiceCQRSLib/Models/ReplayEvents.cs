using SimpleMessageQueue;

namespace ProductServiceCQRSLib.Models
{
    public class ReplayEvents : IMessage<ReplayEvents>
    {
        public readonly long From;
        public readonly long? To;

        public ReplayEvents(long from, long? to = null)
        {
            From = from;
            To = to;
        }

        public ReplayEvents Clone()
        {
            return new ReplayEvents(From, To);
        }
    }
}