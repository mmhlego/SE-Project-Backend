using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using MyOnlineShop.Data;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;
using MyOnlineShop.Models.apimodel;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using System.Security.Claims;
using FsCheck;

namespace MyOnlineShop.Controllers
{
	public class ProductController : ControllerBase
	{
		private MyShopContext _context;
		public ProductController(MyShopContext context)
		{
			_context = context;

		}

		[HttpGet]
		[Route("products/")]
		public ActionResult GetAllProducts(ProductPageGetRequestModel p1)

		{

			List<ProductPrice> product1 = _context.productPrices.ToList();

			if (p1.available == true)
			{
				product1 = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount > 0).ToList();
			}
			else
			{
				product1 = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount == 0).ToList();
			}

			if (p1.catagory != null)
			{
				List<ProductPrice> productPrices = new List<ProductPrice>();
				foreach (ProductPrice p in product1)
				{
					var item = _context.Products.SingleOrDefault(x => x.ID == p.ProductID);
					if (item.Category == p1.catagory)
					{
						productPrices.Add(p);
					}
				}
				product1 = productPrices;
			}
			List<ProductPrice> products = new List<ProductPrice>();

			if ((p1.page * p1.productsPerPage) - p1.productsPerPage < product1.Count)
			{
				if (p1.page * p1.productsPerPage > product1.Count)
				{
					products = product1.GetRange((p1.page * p1.productsPerPage) - p1.productsPerPage, product1.Count);

				}
				else
				{

					products = product1.GetRange((p1.page * p1.productsPerPage) - p1.productsPerPage, p1.productsPerPage);
				}
			}

			List<productModel> productModels = new List<productModel>();
			foreach (ProductPrice productPrice in products)
			{
				var eachproduct = _context.Products.SingleOrDefault(p => p.ID == productPrice.ProductID);
				productModel p = new productModel()
				{
					id = eachproduct.ID,
					image = eachproduct.Image,
					name = eachproduct.Name,
					category = eachproduct.Category,
					description = eachproduct.Description,
					dislikes = eachproduct.dislikes,
					likes = eachproduct.likes
				};
				productModels.Add(p);
			}

			productsModel m = new productsModel()
			{
				page = p1.page,
				productsPerPage = p1.productsPerPage,
				products = productModels
			};

			return Ok(m);

		}


		[HttpPost]
		[Route("products/")]
		[Authorize]
		public ActionResult AddProduct([FromBody] ProductPagePostRequestModel p1)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				// Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

				string username = User.FindFirstValue(ClaimTypes.Name);

				if (username == null)
				{
					return Unauthorized(User);
				}
				var user = _context.users.SingleOrDefault(u => u.UserName == username);
				var accessLevel = user.AccessLevel.ToLower();

				if (accessLevel != "seller" && accessLevel != "admin")
				{
					return Forbid();
				}


				Product productToAdd = new Product()
				{

					ID = Guid.NewGuid(),
					Category = p1.category,
					Name = p1.name,
					Image = p1.image,
					Description = p1.description,
					likes = 0,
					dislikes = 0

				};

				productModel pmod = new productModel()
				{
					id = productToAdd.ID,
					category = productToAdd.Category,
					name = productToAdd.Name,
					image = productToAdd.Image,
					description = productToAdd.Description,
					likes = 0,
					dislikes = 0
				};
				_context.Add(productToAdd);
				_context.SaveChanges();
				return Ok(pmod);










			}

			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

		}



		[HttpGet]
		[Route("products/{id:Guid}")]
		public ActionResult GetProduct(Guid id)
		{
			try
			{
				var products = _context.Products.SingleOrDefault((p) => p.ID == id);
				var p1 = new productModel()
				{
					category = products.Category,
					description = products.Description,
					id = products.ID,
					dislikes = products.dislikes,
					likes = products.likes,
					image = products.Image,
					name = products.Name

				};
				if (products == null)
				{
					return NotFound();
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				return Ok(p1);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}



		[HttpDelete]
		[Route("products/{id:Guid}")]
		[Authorize]
		public ActionResult DeleteProduct(Guid id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var username = User.FindFirstValue(ClaimTypes.Name);
				var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;


				if (userId != null)
				{
					var user = _context.users.SingleOrDefault(u => u.ID == userId);
					var accessLevel = user.AccessLevel.ToLower();

					if (accessLevel == "admin")
					{

						var products = _context.Products.SingleOrDefault((p) => p.ID == id);
						if (products == null)
						{
							return NotFound();
						}

						var Productprice = _context.productPrices.Where(p => p.ProductID == id).ToList();
						if (Productprice == null)
						{
							return NotFound();
						}
						foreach (var p in Productprice)
						{
							p.Amount = 0;
							_context.Update(p);
							_context.SaveChanges();
						}
						var p1 = new productModel()
						{
							category = products.Category,
							description = products.Description,
							id = products.ID,
							dislikes = products.dislikes,
							likes = products.likes,
							image = products.Image,
							name = products.Name

						};
						return Ok(p1);
					}
					else
					{
						return Forbid();
					}
				}
				else
				{
					return Unauthorized();
				}
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}



		[HttpPut]
		[Route("products/{id:Guid}")]
		[Authorize]
		public ActionResult putProduct(Guid id, [FromBody] ProductPagePostRequestModel p1)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				//Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
				var username = User.FindFirstValue(ClaimTypes.Name);
				var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;


				if (userId != null)
				{
					var user = _context.users.SingleOrDefault(u => u.ID == userId);
					var accessLevel = user.AccessLevel.ToLower();
					if (accessLevel == "seller" || accessLevel == "admin")
					{

						var product = _context.Products.Where(p => p.ID == id).Single();
						if (product != null)
						{
							if (p1.name != null) { product.Name = p1.name; }

							if (p1.category != null) { product.Category = p1.category; }

							if (p1.description != null) { product.Description = p1.description; }

							if (p1.image != null) { product.Image = p1.image; }

							_context.Update(product);
							_context.SaveChanges();
							var p2 = new productModel()
							{
								category = p1.category,
								description = p1.description,
								id = id,
								dislikes = 0,
								likes = 0,
								image = p1.image,
								name = p1.name

							};

							return Ok(p2);
						}
						else
						{
							return NotFound();
						}
					}
					else
					{

						return Forbid();
					}
				}
				else
				{
					return Unauthorized();
				}
			}

			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

		}



		[HttpPut]
		[Route("products/{id:Guid}/likes")]
		public ActionResult putProductlike(Guid id, [FromBody] likeModel l)
		{
			try
			{
				var product = _context.Products.Where(p => p.ID == id).Single();
				if (product == null)
					return NotFound();
				else
				{

					if (l.like == true)
					{
						product.likes = product.likes + 1;

					}
					else
					{
						product.dislikes = product.dislikes + 1;

					}
					_context.Update(product);
					_context.SaveChanges();
					if (!ModelState.IsValid)
					{
						return BadRequest(ModelState);
					}

					productModel products = new productModel()
					{
						category = product.Category,
						description = product.Description,
						id = product.ID,
						dislikes = product.dislikes,
						likes = product.likes,
						image = product.Image,
						name = product.Name


					};

					return Ok(products);

				}
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

		}


	}

}
