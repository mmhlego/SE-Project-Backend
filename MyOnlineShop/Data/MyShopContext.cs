using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Models;

namespace MyOnlineShop.Data
{
	public class MyShopContext : DbContext
	{


		public MyShopContext(DbContextOptions<MyShopContext> options) : base(options)
		{

		}


		public DbSet<Order> orders { get; set; }

		public DbSet<Product> Products { get; set; }
		public DbSet<User> users { get; set; }
		public DbSet<Cart> cart { get; set; }
		public DbSet<Comment> comment { get; set; }

		public DbSet<Customer> customer { get; set; }
		public DbSet<DiscountToken> tokens { get; set; }
		public DbSet<ProductPrice> productPrices { get; set; }
		public DbSet<RequestedProducts> requestedProducts { get; set; }
		public DbSet<Seller> sellers { get; set; }
		public DbSet<Stats> stats { get; set; }
		public DbSet<DiscountToken> discountTokens { get; set; }



		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);
		}
	}
}
