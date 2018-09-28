using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class AttachmentsProfile : Profile
    {
        public AttachmentsProfile()
        {
            CreateMap<string, Attachment>()
                .ForMember(e => e.Uri, opt => opt.MapFrom(e => e))
                .ForMember(e => e.AttachmentId, opt => opt.Ignore());

            CreateMap<Attachment, Dto.Attachments.Attachment>();
        }
    }
}
