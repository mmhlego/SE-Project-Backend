using System.ComponentModel.DataAnnotations;


namespace MyOnlineShop.Models
{
    public class Stats
    {
        [Key]
        public Guid Id { get; set; }
        public Guid productId { get; set; }
        public Guid sellerId { get; set; }
        public DateTime date { get; set; }
        public int amount { get; set; }
        public double price { get; set; }
    }
}
