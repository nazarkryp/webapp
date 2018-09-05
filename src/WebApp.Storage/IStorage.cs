using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using WebApp.Storage.Models;

namespace WebApp.Storage
{
    public interface IStorage<TKey>
    {
        Task<IUploadResult<TKey>> UploadAsync(Stream stream);

        Task<Dictionary<string, string>> RemoveAsync(IEnumerable<TKey> objectIds);
    }
}
