using System;
using System.Collections.Generic;
using System.Text;

namespace WebApp.Dto
{
    public class MediaCreateOptions
    {
        public string ObjectId { get; set; }

        public string OriginalUri { get; set; }

        public string Thumbnail { get; set; }

        public string Small { get; set; }

        public string Description { get; set; }
    }
}
