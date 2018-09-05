using WebApp.Dto;

using AutoMapper;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class MediaProfile : Profile
    {
        public MediaProfile()
        {
            CreateMap<MediaCreateOptions, Domain.Entities.Media>()
                .ForMember(e => e.MediaId, opt => opt.Ignore());

            CreateMap<Domain.Entities.Media, Media>();
        }
    }
}
