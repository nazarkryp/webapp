using System;
using System.Collections.Generic;

namespace WebApp.Repositories.EntityFramework.Binding.Models
{
    public class Movie
    {
        public int MovieId { get; set; }

        public string Title { get; set; }

        public string Uri { get; set; }

        public string Description { get; set; }

        public DateTime? Date { get; set; }

        public TimeSpan? Duration { get; set; }

        public int StudioId { get; set; }

        public Studio Studio { get; set; }

        public ICollection<Attachment> Attachments { get; set; }

        public ICollection<MovieModel> MovieModels { get; set; }

        public ICollection<MovieCategory> MovieCategories { get; set; }
    }
}
