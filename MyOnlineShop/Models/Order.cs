using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
    public class Order
    {
        [Key]
        public Guid ID { get; set; }
        public Guid CartID { get; set; }
        public int Amount { get; set; }
        public Guid ProductPriceID { get; set; }
        [ForeignKey("ProductPriceID")]
        public ProductPrice product { get; set; }
        [ForeignKey("CartID")]
        public Cart cart { get; set; }
      

    }
}

