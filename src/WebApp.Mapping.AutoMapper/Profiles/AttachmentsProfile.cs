using AutoMapper;

using WebApp.Domain.Entities;
using WebApp.Studios;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class AttachmentsProfile : Profile
    {
        public AttachmentsProfile()
        {
            CreateMap<IAttachment, Attachment>()
                .ForMember(e => e.AttachmentId, opt => opt.Ignore());
        }
    }
}
