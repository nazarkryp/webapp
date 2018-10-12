using System.Threading.Tasks;

using WebApp.Dto.Models;

namespace WebApp.Services.Models
{
    public interface IModelService
    {
        Task<Model> GetModelByIdAsync(int modelId);
    }
}
