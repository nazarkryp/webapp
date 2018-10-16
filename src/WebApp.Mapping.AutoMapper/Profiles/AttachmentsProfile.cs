using AutoMapper;

using WebApp.Domain.Entities;

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
            //CreateMap<Attachment, Dto.Attachments.Attachment>()
            //    .ForMember(e => e.Uri, opt => opt.ResolveUsing(e => "https://instagram.flwo1-1.fna.fbcdn.net/vp/ee45e6d8512edad0dbd32d334cc23857/5C516B74/t51.2885-15/e35/37029408_207310096778072_4787454093272547328_n.jpg"));
        }
    }
}
