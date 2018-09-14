using AutoMapper;
using WebApp.Domain.Entities;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    public class StudioProfile : Profile
    {
        public StudioProfile()
        {
            CreateMap<Studio, Binding.Models.Studio>()
                .ForMember(e => e.Movies, opt => opt.Ignore());

            CreateMap<Binding.Models.Studio, Studio>();

            CreateMap<SyncDetails, Binding.Models.SyncDetails>()
                .ForMember(e => e.Studio, opt => opt.Ignore())
                .ForMember(e => e.StudioId, opt => opt.MapFrom(s => s.Studio.StudioId));

            CreateMap<Binding.Models.SyncDetails, SyncDetails>();
        }
    }
}
