using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class ModelProfile : Profile
    {
        public ModelProfile()
        {
            CreateMap<IModel, Model>()
                .ForMember(e => e.ModelId, opt => opt.Ignore());
        }
    }
}
