using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<string, Model>()
                .ForMember(e => e.Name, opt => opt.MapFrom(e => e))
                .ForMember(e => e.ModelId, opt => opt.Ignore());

            CreateMap<Model, Dto.Models.Model>();
        }
    }
}
