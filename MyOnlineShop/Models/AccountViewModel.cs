using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
    public class RegisterViewModels
    {
        [Required(ErrorMessage = "You Must {0}")]
        [MaxLength(50)]
        [Display(Name = "Enter UserName")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "You Must {0}")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Display(Name = "Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "You Must {0}")]
        [MaxLength(50)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Repeat password")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "You Must {0}")]
        [MaxLength(50)]
        [EmailAddress]
        [Display(Name = "Enter Email Address")]
        public string Enail { get; set; }

        [Required(ErrorMessage = "You Must {0}")]
        [MaxLength(50)]
        [Display(Name = "Enter Home Address")]
        public string Address { get; set; }
    }
}
