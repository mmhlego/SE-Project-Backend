using Microsoft.EntityFrameworkCore;
using MyOnlineShop.Models;


namespace MyOnlineShop.Data
{
    public class MyShopContex : DbContext
    {


        public MyShopContex(DbContextOptions<MyShopContex> options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Cart> cart { get; set; }
        public DbSet<Comment> comment { get; set; }

        public DbSet<Customer> customer { get; set; }
        public DbSet<GiftCard> giftCards { get; set; }
        public DbSet<ProductPrice> productPrices { get; set; }
        public DbSet<RequestedProducts> requestedProducts { get; set; }
        public DbSet<Seller> sellers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
           
            base.OnModelCreating(modelBuilder);
        }
    }
}
