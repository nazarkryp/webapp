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

        public TimeSpan Duration { get; set; }

        public IEnumerable<StudioAttachment> Attachments { get; set; }

        public IEnumerable<StudioModel> Models { get; set; }
    }
}
