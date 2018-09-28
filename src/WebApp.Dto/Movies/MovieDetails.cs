using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Dto.Attachments;
using WebApp.Dto.Models;

namespace WebApp.Dto.Movies
{
    public class MovieDetails
    {
        public int MovieId { get; set; }

        public string Uri { get; set; }

        public string Description { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime Date { get; set; }

        public IEnumerable<Attachment> Attachments { get; set; }

        public IEnumerable<Model> Models { get; set; }
    }
}
