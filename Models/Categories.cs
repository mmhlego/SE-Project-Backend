using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
    public class Categories
    {
        [Key]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public List<Product> products { get; set; }
    }
}
