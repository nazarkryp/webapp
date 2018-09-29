using System.Collections.Generic;
using System.Threading.Tasks;

using WebApp.Dto.Categories;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;

namespace WebApp.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categories;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categories, IMapper mapper)
        {
            _categories = categories;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            var categories = await _categories.FindCategoriesAsync();

            return _mapper.Map<IEnumerable<Category>>(categories);
        }
    }
}
