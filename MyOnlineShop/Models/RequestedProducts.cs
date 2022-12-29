using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
	public class RequestedProducts
	{
		[Key]
		public Guid ID { get; set; }

		public Guid UserID { get; set; }

		public Guid ProductID { get; set; }

		[ForeignKey("UserID")]
		public User user { get; set; }

		[ForeignKey("ProductID")]
		public Product product { get; set; }
	}
}
