using System.Collections.Generic;

namespace WebApp.Repositories.EntityFramework.Binding.Models
{
    public class Studio
    {
        public int StudioId { get; set; }

        public string Name { get; set; }

        // public int? SyncDetailsId { get; set; }

        public SyncDetails SyncDetails { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}
