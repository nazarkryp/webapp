using System.Collections.Generic;

namespace WebApp.Dto.Common
{
    public class DataResponse<T>
    {
        public DataResponse(IEnumerable<T> data)
        {
            Data = data;
        }

        public IEnumerable<T> Data { get; set; }
    }
}
