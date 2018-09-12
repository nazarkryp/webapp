using System;
using System.Collections.Generic;

namespace WebApp.Studios
{
    public interface IMovie
    {
        string Title { get; set; }

        string Uri { get; set; }

        string Description { get; set; }

        DateTime Date { get; set; }

        TimeSpan Duration { get; set; }

        IEnumerable<IAttachment> Attachments { get; set; }
    }
}
