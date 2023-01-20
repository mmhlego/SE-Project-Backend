using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models;

namespace MyOnlineShop.Controllers
{
	public class SeedData : ControllerBase
	{
		MyShopContext _context;
		public SeedData(MyShopContext contex)
		{
			_context = contex;
		}

		[HttpPost]
		[Route("add/user")]
		public IActionResult AddUser(int Amount, string type)
		{

			for (int i = 0; i < Amount; i++)
			{

				Random random = new Random();
				int pass = random.Next();
				int numb = random.Next();
				string imageurl = "";
				if (i % 2 == 0)
				{
					imageurl = "https://randomuser.me/api/portraits/women/" + (i) + ".jpg";
				}
				else
				{
					imageurl = "https://randomuser.me/api/portraits/men/" + (i) + ".jpg";
				}


				var user = new User()
				{

					ID = Guid.NewGuid(),
					FirstName = type + numb,
					UserName = type + numb,
					AccessLevel = type,
					BirthDate = DateTime.Now,
					Email = "SellerEmail@gmail.com",
					ImageUrl = imageurl,
					IsApproved = true,
					LastName = type + numb,
					Password = pass.ToString(),
					PhoneNumber = "0914697" + i + "491"
				};
				_context.users.Add(user);
				_context.SaveChanges();

				if (type.ToLower() == "seller")
				{
					var seller = new Seller()
					{
						ID = Guid.NewGuid(),
						UserId = user.ID,
						Address = "new address",
						Information = "seller information",
						likes = 0,
						dislikes = 0
					};
					_context.sellers.Add(seller);
					_context.SaveChanges();
				}
				else if (type.ToLower() == "customer")
				{
					var customer = new Customer()
					{
						ID = Guid.NewGuid(),
						UserId = user.ID,
						Address = "customer Address",
						Balance = 0
					};
					_context.customer.Add(customer);
					_context.SaveChanges();
				}
			}
			return Ok();
		}

		[HttpPost]
		[Route("add/Product")]
		public IActionResult AddProduct(int Amount, string category, string imageurl)
		{


			List<Seller> sellers = _context.sellers.ToList();
			for (int i = 0; i < Amount; i++)
			{

				Random random = new Random();
				int numb = random.Next();
				double price = random.NextDouble() * 10000000;
				int randseller = random.Next(sellers.Count);

				Product productToAdd = new Product()
				{

					ID = Guid.NewGuid(),
					Category = category,
					Name = category + numb,
					Image = imageurl,
					Description = "product description",
					likes = 0,
					dislikes = 0

				};
				_context.Products.Add(productToAdd);
				_context.SaveChanges();

				var productPrice = new ProductPrice()
				{
					Price = price,
					PriceHistory = "[]",
					Amount = 10,
					Discount = "",
					SellerID = sellers[randseller].ID,
					ProductID = productToAdd.ID,
					ID = Guid.NewGuid()

				};
				_context.productPrices.Add(productPrice);
				_context.SaveChanges();
			}

			return Ok();
		}


		[HttpPost]
		[Route("add/Comment")]
		public IActionResult AddComment(int Amount)
		{

			List<Customer> customers = _context.customer.ToList();
			List<Product> products = _context.Products.ToList();
			for (int i = 0; i < Amount; i++)
			{

				Random random = new Random();
				int numb = random.Next();
				int randProduct = random.Next(products.Count);
				int randCust = random.Next(customers.Count);

				var comment = new Comment()
				{
					Id = Guid.NewGuid(),
					ProductId = products[randProduct].ID,
					UserId = customers[randCust].UserId,
					likes = 0,
					dislikes = 0,
					Text = "this is Test Comment",
					SentDate = DateTime.Now
				};
				_context.comment.Add(comment);
			}

			_context.SaveChanges();

			return Ok();
		}


		[HttpPost]
		[Route("add/Cart")]
		public IActionResult AddtoCart(int Amount, string status)
		{


			List<Customer> customers = _context.customer.ToList();
			List<Product> products = _context.Products.ToList();
			for (int i = 0; i < Amount; i++)
			{

				Random random = new Random();
				int randProduct = random.Next(products.Count);
				int randCust = random.Next(customers.Count);

				var cart = new Cart()
				{
					ID = Guid.NewGuid(),
					CustomerID = customers[randCust].ID,
					Status = status,
					TotalPrice = 0,
					Description = "Cart Description",
					UpdateDate = DateTime.Now,
				};
				_context.cart.Add(cart);
			}
			_context.SaveChanges();
			return Ok();
		}

		[HttpPost]
		[Route("add/Order")]
		public IActionResult AddOrder(int Amount)
		{

			List<Cart> carts = _context.cart.ToList();
			List<ProductPrice> productPrices = _context.productPrices.ToList();
			for (int i = 0; i < Amount; i++)
			{

				Random random = new Random();
				int randProduct = random.Next(productPrices.Count);
				int randCart = random.Next(carts.Count);

				var order = new Order()
				{
					ID = Guid.NewGuid(),
					Amount = 2,
					CartID = carts[randCart].ID,
					ProductPriceID = productPrices[randProduct].ID
				};
				_context.orders.Add(order);

				var cart = _context.cart.Where(c => c.ID == carts[randCart].ID).Single();
				cart.TotalPrice = order.Amount * productPrices[randProduct].Price;
				_context.Update(cart);
				_context.SaveChanges();


			}

			return Ok();
		}
	}
}
