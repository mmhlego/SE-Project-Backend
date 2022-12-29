using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace MyOnlineShop.Models
{
	public class ProductPrice
	{
		[Key]
		public Guid ID { get; set; }
		public Guid SellerID { get; set; }
		public Guid ProductID { get; set; }
		public int Amount { get; set; }
		public Double Price { get; set; }
		public String PriceHistory { get; set; }
		public String Discount { get; set; }
		[ForeignKey("ProductID")]
		public Product product { get; set; }
		List<Cart> carts { get; set; }



	}
}

