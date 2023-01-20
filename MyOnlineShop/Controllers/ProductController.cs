using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using MyOnlineShop.Services;
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

			List<ProductPrice> prices = _context.productPrices.ToList();

			if (p1.available == true)
			{
				prices = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount > 0).ToList();
			}
			else
			{
				prices = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo).ToList();
			}

			if (p1.category != null)
			{
				List<ProductPrice> productPrices = new List<ProductPrice>();
				foreach (ProductPrice p in prices)
				{
					var item = _context.Products.SingleOrDefault(x => x.ID == p.ProductID);
					if (item.Category == p1.category)
					{
						productPrices.Add(p);
					}
				}
				prices = productPrices;
			}
			List<ProductPrice> products = new List<ProductPrice>();

			var totalPages = (int)Math.Ceiling(Convert.ToDecimal(prices.Count) / Convert.ToDecimal(p1.productsPerPage));
			p1.page = Math.Min(totalPages, p1.page);
			var start = Math.Max((p1.page - 1) * p1.productsPerPage, 0);
			var end = Math.Min(p1.page * p1.productsPerPage, prices.Count);
			var count = Math.Max(end - start, 0);

			products = prices.GetRange(start, count);

			List<productModel> productModels = new List<productModel>();
			foreach (ProductPrice productPrice in products)
			{
				var eachProduct = _context.Products.SingleOrDefault(p => p.ID == productPrice.ProductID);
				productModel p = new productModel()
				{
					id = eachProduct.ID,
					image = eachProduct.Image,
					name = eachProduct.Name,
					category = eachProduct.Category,
					description = eachProduct.Description,
					dislikes = eachProduct.dislikes,
					likes = eachProduct.likes
				};
				productModels.Add(p);
			}

			var m = new Pagination<productModel>()
			{
				page = p1.page,
				perPage = p1.productsPerPage,
				totalPages = totalPages,
				data = productModels
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
					Logger.LoggerFunc("products", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, BadRequest(ModelState));
					return BadRequest(ModelState);
				}

				// Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());

				string username = User.FindFirstValue(ClaimTypes.Name);

				if (username == null)
				{
					Logger.LoggerFunc("products", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status401Unauthorized));
					return StatusCode(StatusCodes.Status401Unauthorized);
				}
				var user = _context.users.SingleOrDefault(u => u.UserName == username);
				var accessLevel = user.AccessLevel.ToLower();

				if (accessLevel != "seller" && accessLevel != "admin")
				{
					Logger.LoggerFunc("products", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status403Forbidden));
					return StatusCode(StatusCodes.Status403Forbidden);
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
				Logger.LoggerFunc("products", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, pmod);
				return Ok(pmod);
			}
			catch
			{
				Logger.LoggerFunc("products", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpGet]
		[Route("products/{id:Guid}")]
		public ActionResult GetProduct(Guid id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var product = _context.Products.ToList();
				Product p2 = null;
				int i = 0;
				foreach (var t in product)
				{
					if (t.ID == id)
					{
						p2 = product.Single(x => x.ID == id);
						i++;
					}
				}

				if (p2 == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}

				var p3 = new productModel()
				{
					category = p2.Category,
					description = p2.Description,
					id = p2.ID,
					dislikes = p2.dislikes,
					likes = p2.likes,
					image = p2.Image,
					name = p2.Name

				};

				return Ok(p3);
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
					Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, BadRequest(ModelState));
					return BadRequest(ModelState);
				}

				string username = User.FindFirstValue(ClaimTypes.Name);

				if (username == null)
				{
					Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, StatusCode(StatusCodes.Status401Unauthorized));
					return StatusCode(StatusCodes.Status401Unauthorized);
				}
				var user = _context.users.SingleOrDefault(u => u.UserName == username);
				var accessLevel = user.AccessLevel.ToLower();

				if (accessLevel != "admin")
				{
					Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, StatusCode(StatusCodes.Status403Forbidden));
					return StatusCode(StatusCodes.Status403Forbidden);
				}
				else
				{

					var product = _context.Products.ToList();
					Product p2 = null;
					int i = 0;
					foreach (var t in product)
					{
						if (t.ID == id)
						{
							p2 = product.Single(x => x.ID == id);
							i++;
						}
					}

					if (p2 == null)
					{
						Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, StatusCode(StatusCodes.Status404NotFound));
						return StatusCode(StatusCodes.Status404NotFound);
					}
					var productPrice = _context.productPrices.ToList();

					ProductPrice pp = null;
					int j = 0;
					foreach (var t in productPrice)
					{
						if (t.ProductID == id)
						{
							pp = productPrice.Single(x => x.ProductID == id);
							j++;
						}
					}

					if (pp == null)
					{
						Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, StatusCode(StatusCodes.Status404NotFound));
						return StatusCode(StatusCodes.Status404NotFound);
					}

					pp.Amount = 0;
					_context.Update(pp);
					_context.SaveChanges();

					var p1 = new productModel()
					{
						category = p2.Category,
						description = p2.Description,
						id = p2.ID,
						dislikes = p2.dislikes,
						likes = p2.likes,
						image = p2.Image,
						name = p2.Name

					};
					Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, p1);
					return Ok(p1);
				}
			}
			catch
			{
				Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								id, StatusCode(StatusCodes.Status500InternalServerError));
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
					Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, BadRequest(ModelState));
					return BadRequest(ModelState);
				}
				//Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
				var username = User.FindFirstValue(ClaimTypes.Name);


				if (username != null)
				{
					var userId = _context.users.SingleOrDefault(u => u.UserName == username).ID;

					var user = _context.users.SingleOrDefault(u => u.ID == userId);
					var accessLevel = user.AccessLevel.ToLower();
					if (accessLevel == "seller" || accessLevel == "admin")
					{

						var product = _context.Products.ToList();
						Product p3 = null;
						int i = 0;
						foreach (var t in product)
						{
							if (t.ID == id)
							{
								p3 = product.Single(x => x.ID == id);
								i++;
							}
						}

						if (p3 == null)
						{
							Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status404NotFound));
							return StatusCode(StatusCodes.Status404NotFound);
						}
						else
						{
							if (p1.name != null) { p3.Name = p1.name; }

							if (p1.category != null) { p3.Category = p1.category; }

							if (p1.description != null) { p3.Description = p1.description; }

							if (p1.image != null) { p3.Image = p1.image; }

							_context.Update(p3);
							_context.SaveChanges();
							var p2 = new productModel()
							{
								category = p3.Category,
								description = p3.Description,
								id = id,
								dislikes = p3.dislikes,
								likes = p3.likes,
								image = p3.Image,
								name = p3.Name
							};
							Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, p2);
							return Ok(p2);
						}

					}
					else
					{
						Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status403Forbidden));
						return StatusCode(StatusCodes.Status403Forbidden);
					}
				}
				else
				{
					Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status401Unauthorized));
					return StatusCode(StatusCodes.Status401Unauthorized);
				}
			}
			catch
			{
				Logger.LoggerFunc($"products/{id:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p1, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}



		[HttpPut]
		[Route("products/{id:Guid}/likes")]
		public ActionResult putProductlike(Guid id, [FromBody] likeModel l)
		{
			try
			{
				var username = User.FindFirstValue(ClaimTypes.Name);


				if (username != null)
				{


					var user = _context.users.SingleOrDefault(u => u.UserName == username);
					var accessLevel = user.AccessLevel.ToLower();
					if (accessLevel != "customer")
					{
						Logger.LoggerFunc($"products/{id:Guid}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								l, StatusCode(StatusCodes.Status403Forbidden));
						return StatusCode(StatusCodes.Status403Forbidden);
					}
				}
				else
				{
					Logger.LoggerFunc($"products/{id:Guid}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								l, StatusCode(StatusCodes.Status401Unauthorized));
					return StatusCode(StatusCodes.Status401Unauthorized);
				}
				var products = _context.Products.ToList();
				Product product = null;
				int i = 0;
				foreach (var t in products)
				{
					if (t.ID == id)
					{
						product = products.Single(x => x.ID == id);
						i++;
					}
				}

				if (product == null)
				{
					Logger.LoggerFunc($"products/{id:Guid}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								l, StatusCode(StatusCodes.Status404NotFound));
					return StatusCode(StatusCodes.Status404NotFound);
				}
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
						Logger.LoggerFunc($"products/{id:Guid}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								l, BadRequest(ModelState));
						return BadRequest(ModelState);
					}

					productModel productss = new productModel()
					{
						category = product.Category,
						description = product.Description,
						id = product.ID,
						dislikes = product.dislikes,
						likes = product.likes,
						image = product.Image,
						name = product.Name


					};

					Logger.LoggerFunc($"products/{id:Guid}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								l, productss);
					return Ok(productss);

				}
			}
			catch
			{
				Logger.LoggerFunc($"products/{id:Guid}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								l, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
