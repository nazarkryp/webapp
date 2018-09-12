using System;
using System.Collections.Generic;

namespace WebApp.Studios.Brazzers.Models
{
    internal class Movie : IMovie
    {
        public string Title { get; set; }

        public string Uri { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan Duration { get; set; }

        public IEnumerable<IAttachment> Attachments { get; set; }
    }
}
