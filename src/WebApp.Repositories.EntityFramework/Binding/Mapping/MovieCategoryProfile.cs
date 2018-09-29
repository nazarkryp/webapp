using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using WebApp.Repositories.EntityFramework.Binding.Models;
using Category = WebApp.Domain.Entities.Category;

namespace WebApp.Repositories.EntityFramework.Binding.Mapping
{
    internal class MovieCategoryProfile : Profile
    {
        public MovieCategoryProfile()
        {
            // CreateMap<Category, MovieCategory>();
        }
    }
}
