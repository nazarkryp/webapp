using System.Collections.Generic;

namespace WebApp.Repositories.EntityFramework.Binding.Models
{
    public class Model
    {
        public int ModelId { get; set; }

        public string Name { get; set; }

        public ICollection<MovieModel> MovieModel { get; set; }
    }
}
