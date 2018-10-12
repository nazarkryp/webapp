using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using WebApp.Domain.Entities;
using WebApp.Mapping;
using WebApp.Repositories.EntityFramework.Context;
using WebApp.Repositories.Repositories;

namespace WebApp.Repositories.EntityFramework.Repositories
{
    public class ModelRepository : GenericRepository<Binding.Models.Model>, IModelRepository
    {
        private readonly IMapper _mapper;

        public ModelRepository(IDbContext context, IMapper mapper)
            : base(context)
        {
            _mapper = mapper;
        }

        public async Task<Model> FindModelAsync(int modelId)
        {
            var model = await Context.Set<Binding.Models.Model>().FirstOrDefaultAsync(e => e.ModelId == modelId);

            return _mapper.Map<Model>(model);
        }
    }
}
