using AutoMapper;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, Binding.Models.Movie>()
                .ForMember(e => e.StudioId, opt => opt.MapFrom(e => e.Studio.StudioId))
                .ForMember(e => e.Studio, opt => opt.Ignore());

            CreateMap<Binding.Models.Movie, Movie>();
        }
    }
}
