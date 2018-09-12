using System.Collections.Generic;

namespace WebApp.Sync.Models
{
    internal class SyncObject<T>
    {
        public int PageIndex { get; set; }

        public IEnumerable<T> Items { get; set; }
    }
}
