using AutoMapper;

using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class AttachmentsProfile : Profile
    {
        public AttachmentsProfile()
        {
            CreateMap<Attachment, Domain.Entities.Attachment>();

            CreateMap<Domain.Entities.Attachment, Attachment>()
                .ForMember(e => e.Movie, opt => opt.Ignore())
                .ForMember(e => e.MovieId, opt => opt.Ignore());
        }
    }
}
