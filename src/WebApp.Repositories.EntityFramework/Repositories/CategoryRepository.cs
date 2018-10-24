using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class CategoryRepository : GenericRepository<Binding.Models.Category>, ICategoryRepository
    {
        private readonly IMapper _mapper;

        public CategoryRepository(IDbContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<IEnumerable<Category>> FindCategoriesAsync()
        {
            var categories = await Context.Set<Binding.Models.Category>().ToListAsync();

            return _mapper.Map<IEnumerable<Category>>(categories);
        }

        public async Task<Category> FindCategoryAsync(int categoryId)
        {
            var category = await Context.Set<Binding.Models.Category>().FirstOrDefaultAsync(e=>e.CategoryId == categoryId);

            return _mapper.Map<Category>(category);
        }

        public async Task<IEnumerable<Category>> FindTopCategoriesAsync()
        {
            var categories = await Context.Set<Binding.Models.TopCategory>()
                .Include(e => e.Category)
                .Select(e => e.Category)
                .ToListAsync();

            return _mapper.Map<IEnumerable<Category>>(categories);
        }
    }
}
