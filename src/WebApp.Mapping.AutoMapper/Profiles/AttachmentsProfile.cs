using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class AttachmentsProfile : Profile
    {
        public AttachmentsProfile()
        {
            CreateMap<StudioAttachment, Attachment>()
                .ForMember(e => e.AttachmentId, opt => opt.Ignore());

            CreateMap<Attachment, Dto.Attachments.Attachment>();
        }
    }
}
