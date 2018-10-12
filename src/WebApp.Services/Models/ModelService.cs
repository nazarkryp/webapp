using System.Threading.Tasks;

using WebApp.Dto.Models;
using WebApp.Mapping;
using WebApp.Repositories.Repositories;
using WebApp.Services.Exceptions;

namespace WebApp.Services.Models
{
    public class ModelService : IModelService
    {
        private readonly IModelRepository _models;
        private readonly IMapper _mapper;

        public ModelService(IModelRepository models, IMapper mapper)
        {
            _models = models;
            _mapper = mapper;
        }

        public async Task<Model> GetModelByIdAsync(int modelId)
        {
            var model = await _models.FindModelAsync(modelId);

            if (model == null)
            {
                throw new ResourceNotFoundException("Model not found");
            }

            return _mapper.Map<Model>(model);
        }
    }
}
