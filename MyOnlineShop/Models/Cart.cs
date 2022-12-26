using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
    public class Cart
    {
       [Key]
       public Guid ID{ get; set; }
        
        [Required]
        public string Status { get; set; }

       [Required]
       public string Discription { get; set; }

        [Required]
        public DateTime UpdateDate { get; set; }
        public Guid ProductId { get; set; }

        [ForeignKey("ProductId")]
        public List<Product> Products { get; set; }

    }
}
