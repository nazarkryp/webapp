using AutoMapper;
using WebApp.Repositories.EntityFramework.Binding.Models;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class MediaProfile : Profile
    {
        public MediaProfile()
        {
            CreateMap<Media, Domain.Entities.Media>();
            CreateMap<Domain.Entities.Media, Media>();
        }
    }
}
