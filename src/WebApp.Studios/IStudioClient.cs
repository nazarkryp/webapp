using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApp.Studios
{
    public interface IStudioClient
    {
        string StudioName { get; }

        Task<IEnumerable<IMovie>> GetMoviesAsync(int page);

        IEnumerable<Task<IEnumerable<IMovie>>> GetPages(int? startPage = null);

        Task<IEnumerable<IMovie>> GetPage(int page);

        Task<IPage> EnumeratePages(int? pageNumber = null);
    }
}
