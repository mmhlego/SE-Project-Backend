using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
    public class Verification
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(50)]
        public DateTime ValidTime { get; set; }
    }
}
