namespace ProductServiceCQRSLib.Models
{
    public class ReplayEvents
    {
        public readonly long From;
        public readonly long? To;

        public ReplayEvents(long from, long? to = null)
        {
            From = from;
            To = to;
        }
    }
}