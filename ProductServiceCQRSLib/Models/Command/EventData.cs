using System;
using System.Collections.Generic;
using System.Text;

namespace ProductServiceCQRSLib.Models.Command
{
    public class EventData
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public object Payload { get; set; }
    }
}
