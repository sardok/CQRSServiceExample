using System;

namespace ProductServiceCQRSLib.Models.Command
{
    public class Event
    {
        public long? Id { get; set; }
        public EventData Data { get; set; }
        public long? Seq { get; set; }
        public DateTime? Timestamp { get; set; }
    }
}