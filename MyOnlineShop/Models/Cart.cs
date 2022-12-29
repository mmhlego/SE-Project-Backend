using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyOnlineShop.Models
{
	public class Cart
	{
		[Key]
		public Guid ID { get; set; }

		[Required]
		public Guid CustomerID { get; set; }

		[Required]
		public string Status { get; set; }
		public double TotalPrice { get; set; }
		[Required]
		public string Discription { get; set; }

		[Required]
		public DateTime UpdateDate { get; set; }
		public Guid ProductId { get; set; }

		public List<Product> Products { get; set; }

	}
}
