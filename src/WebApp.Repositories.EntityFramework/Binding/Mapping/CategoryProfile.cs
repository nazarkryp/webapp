using AutoMapper;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, Domain.Entities.Category>();

            CreateMap<Domain.Entities.Category, Category>()
                .ForMember(e => e.MovieCategories, opt => opt.Ignore());
        }
    }
}
