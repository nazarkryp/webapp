using System;
using System.Collections.Generic;

namespace WebApp.Domain.Entities
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; }

        public string Uri { get; set; }

        public string Description { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? Duration { get; set; }

        public Studio Studio { get; set; }

        public IList<Attachment> Attachments { get; set; }

        public IList<Model> Models { get; set; }

        public IList<Category> Categories { get; set; }
    }
}
