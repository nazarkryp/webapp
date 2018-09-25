using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<WebApp.Studios.StudioModel, Model>()
                  .ForMember(e => e.ModelId, opt => opt.Ignore());

            CreateMap<Model, Dto.Models.Model>();
        }
    }
}
