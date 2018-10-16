using System.Linq;

using AutoMapper;

using WebApp.Domain.Entities;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, Binding.Models.Movie>()
                .ForMember(e => e.StudioId, opt =>
                {
                    opt.Condition(e => e.Studio != null);
                    opt.MapFrom(e => e.Studio.StudioId);
                })
                .ForMember(e => e.Attachments, opt =>
                {
                    opt.PreCondition(e => e.Attachments != null && e.Attachments.Any());
                    opt.Condition(e => e.Attachments != null && e.Attachments.Any()); // not working
                    opt.MapFrom(e => e.Attachments);
                })
                .ForMember(e => e.MovieModels, opt => opt.ResolveUsing(e =>
                {
                    return e.Models?.Select(m => new Binding.Models.MovieModel
                    {
                        MovieId = e.MovieId,
                        Model = new Binding.Models.Model
                        {
                            Name = m.Name
                        }
                    }) ?? Enumerable.Empty<Binding.Models.MovieModel>();
                }))
                .ForMember(e => e.MovieCategories, opt =>
                {
                    opt.ResolveUsing(e =>
                    {
                        return e.Categories?.Select(m => new Binding.Models.MovieCategory
                        {
                            MovieId = e.MovieId,
                            Category = new Binding.Models.Category
                            {
                                Name = m.Name
                            }
                        }) ?? Enumerable.Empty<Binding.Models.MovieCategory>();
                    });
                })
                .ForMember(e => e.Studio, opt => opt.Ignore());

            CreateMap<Binding.Models.Movie, Movie>()
                .ForMember(e => e.Categories, opt => opt.MapFrom(e => e.MovieCategories.Select(mc => mc.Category)))
                .ForMember(e => e.Models, opt => opt.MapFrom(e => e.MovieModels.Select(mm => mm.Model)));
        }
    }
}
