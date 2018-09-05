using System;
using System.Collections.Generic;

namespace WebApp.Messaging
{
    public class WebAppEvent : IEvent
    {
        public IEnumerable<string> ObjectIds { get; set; }

        public DateTime RemoveTime { get; set; }
    }
}
