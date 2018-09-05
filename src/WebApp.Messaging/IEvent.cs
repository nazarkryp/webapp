using System;
using System.Collections.Generic;

namespace WebApp.Messaging
{
    public interface IEvent
    {
        IEnumerable<string> ObjectIds { get; set; }

        DateTime RemoveTime { get; set; }
    }
}
