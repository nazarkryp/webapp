using System.Threading.Tasks;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.Repositories
{
    public interface IModelRepository
    {
        Task<Model> FindModelAsync(int modelId);
    }
}
