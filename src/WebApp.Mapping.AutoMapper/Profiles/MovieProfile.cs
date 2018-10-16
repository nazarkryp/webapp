using System.Linq;
using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<StudioMovie, Movie>()
                .ForMember(e => e.Models, opt =>
                {
                    opt.MapFrom(e => e.Models);
                })
                .ForMember(e => e.Categories, opt =>
                {
                    opt.MapFrom(e => e.Categories);
                })
                .ForMember(e => e.MovieId, opt => opt.Ignore())
                .ForMember(e => e.Studio, opt => opt.Ignore());

            CreateMap<Movie, Dto.Movies.Movie>()
                .ForMember(e => e.Models, opt =>
                {
                    opt.Condition(e => e.Models != null && e.Models.Any());
                    opt.MapFrom(e => e.Models);
                })
                .ForMember(e => e.Categories, opt =>
                {
                    opt.Condition(e => e.Categories != null && e.Categories.Any());
                    opt.MapFrom(e => e.Categories);
                });
        }
    }
}
