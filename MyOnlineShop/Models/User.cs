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

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public string ImageUrl { get; set; }
        public string Password { get; set; }
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public DateTime BirthDate { get; set; }
        [Required]
        public string AccessLevel { get; set; }
        public bool Restricted { get; set; }
        [Required]
        public bool IsApproved { get; set; }
        public List<Customer> customers { get; set; }
        public List<Seller> sellers { get; set; }
    }
}
