using System;

namespace WebApp.Repositories.EntityFramework.Binding.Models
{
    public class SyncDetails
    {
        public int SyncDetailsId { get; set; }

        public int StudioId { get; set; }

        public Studio Studio { get; set; }

        public DateTime? LastSyncDate { get; set; }

        public int? LastSyncPage { get; set; }
    }
}
