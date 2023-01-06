using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyOnlineShop.Models
{
	public class Product
	{
		[Key]
		public Guid ID { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public string Image { get; set; }
		public int likes { get; set; }
		public int dislikes { get; set; }
		[Required]
		public string Category { get; set; }

		public List<ProductPrice> Prices { get; set; }
	}
}
