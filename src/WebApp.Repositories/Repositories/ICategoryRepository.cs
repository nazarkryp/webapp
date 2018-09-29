using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.Repositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> FindCategoriesAsync();
    }
}
