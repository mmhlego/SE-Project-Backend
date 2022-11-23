using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
    public class User
    {
        [Key]
        public Guid ID { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(50)]
        [MinLength(8)]
        public string Password { get; set; }
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public DateOnly  BirthDate { get; set; }
        [Required]
        public string AccessLevel { get; set; }
        [Required]
        public bool IsApproved { get; set; }

        public List<Customer> customers { get; set; }
        public List<Seller> sellers { get; set; }
    }
}
