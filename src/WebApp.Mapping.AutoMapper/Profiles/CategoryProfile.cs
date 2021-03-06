﻿using AutoMapper;

using WebApp.Domain.Entities;

namespace WebApp.Mapping.AutoMapper.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<string, Category>()
                .ForMember(e => e.Name, opt => opt.MapFrom(e => e))
                .ForMember(e => e.CategoryId, opt => opt.Ignore());

            CreateMap<Category, Dto.Categories.Category>();
        }
    }
}
