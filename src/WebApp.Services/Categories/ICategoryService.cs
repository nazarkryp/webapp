using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto.Categories;

namespace WebApp.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
    }
}
