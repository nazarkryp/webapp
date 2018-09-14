using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<IMovie, Movie>()
                .ForMember(e => e.Models, opt => opt.MapFrom(e => e.Models))
                .ForMember(e => e.MovieId, opt => opt.Ignore())
                .ForMember(e => e.Studio, opt => opt.Ignore());
        }
    }
}
