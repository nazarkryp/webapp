using System.Collections.Generic;

namespace WebApp.Dto.Common
{
    public class Page<T>
    {
        public IEnumerable<T> Data { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }

        public int Total { get; set; }
    }
}
