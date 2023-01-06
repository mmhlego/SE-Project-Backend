using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
	public class Cart
	{
		[Key]
		public Guid ID { get; set; }
		[Required]
		public Guid CustomerID { get; set; }
		[ForeignKey("CustomerID")]
		public Customer Customer { get; set; }
		[Required]
		public string Status { get; set; }
		public double TotalPrice { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public DateTime UpdateDate { get; set; }
		public List<Order> orders { get; set; }

	}
}
