using System.Collections.Generic;
using System.Linq;

namespace WebApp.Repositories.Common
{
    public class Page<TEntity>
    {
        public Page()
        {
            Data = Enumerable.Empty<TEntity>();
        }

        public IEnumerable<TEntity> Data { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }

        public int Total { get; set; }
    }
}
