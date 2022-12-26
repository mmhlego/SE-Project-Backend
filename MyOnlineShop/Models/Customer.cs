using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
    public class Customer
    {
        [Key]
        public int ID { get; set; }
        public Guid UserId { get; set; }
        [Required]

        public Guid CartId { get; set; }
        public string Address { get; set; }
        [Required]
        public double Balance { get; set; }


        [ForeignKey("CartId")]
        public Cart cart { get; set; }
        

        [ForeignKey("UserId")]
        public User user { get; set; }
    }
}
