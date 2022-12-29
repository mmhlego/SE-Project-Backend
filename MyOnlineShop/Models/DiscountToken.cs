using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
    public class DiscountToken
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Discount { get; set; }
        public Boolean IsEvent { get; set; }


    }
}
