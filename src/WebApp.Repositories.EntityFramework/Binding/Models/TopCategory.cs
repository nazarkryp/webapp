namespace WebApp.Repositories.EntityFramework.Binding.Models
{
    public class TopCategory
    {
        public int TopCategoryId { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }
    }
}
