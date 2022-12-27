using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        
        public Guid UserId { get; set; }
        
        public Guid ProductId { get; set; }

        public DateTime SentDate { get; set; }

        public string Text { get; set; }
        public int likes { get; set; }
        public int dislikes { get; set; }

        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [ForeignKey("UserId")]
        public User user { get; set; }
    }
}
