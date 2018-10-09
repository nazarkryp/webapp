using System;
using System.Collections.Generic;

namespace WebApp.Studios
{
    public class StudioMovie
    {
        public string Title { get; set; }

        public string Uri { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public TimeSpan? Duration { get; set; }

        public IEnumerable<string> Attachments { get; set; }

        public IEnumerable<string> Models { get; set; }

        public IEnumerable<string> Categories { get; set; }
    }
}
