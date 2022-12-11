using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
    public class Product
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Descriptiopn { get; set; }
        [Required]
        public string Url { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }
        [Required]
        public Guid CategoriesID { get; set; }

        public List<ProductPrice> Prices { get; set; }
        [ForeignKey("CategoriesID")]
        public Categories Categories { get; set; }

    }
}
