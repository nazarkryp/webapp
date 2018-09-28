using AutoMapper;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<Model, Domain.Entities.Model>();

            CreateMap<Domain.Entities.Model, Model>()
                .ForMember(e => e.MovieModel, opt => opt.Ignore());
        }
    }
}
