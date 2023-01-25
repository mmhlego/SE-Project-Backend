using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Services;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using Microsoft.CodeAnalysis;
using System.Reflection.Metadata.Ecma335;
using System.Web.Helpers;

namespace MyOnlineShop.Controllers
{
	public class ProfileController : ControllerBase
	{
		private MyShopContext _context;
		public ProfileController(MyShopContext context)
		{
			_context = context;

		}
		[HttpGet]
		[Route("profile/")]
		public ActionResult GetProfiles()
		{
			try
			{
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);

				if (user == null)
				{
					return Unauthorized();
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
					profileModel p = new profileModel()
					{
						id = user.ID,
						username = user.UserName,
						firstName = user.FirstName,
						lastName = user.LastName,
						phoneNumber = user.PhoneNumber,
						email = user.Email,
						profileImage = user.ImageUrl,
						birthDate = user.BirthDate,
						accessLevel = user.AccessLevel,
						restricted = user.Restricted
					};
					return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}


		[HttpPut]
		[Route("profile/")]
		public ActionResult putProfile([FromBody] putprofileModel p)
		{
			try
			{
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);

				if (user == null)
				{
					Logger.LoggerFunc("profile/", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p, Unauthorized());
					return Unauthorized();
				}

                if (p.email != null) {
					var emailcheck = _context.users.SingleOrDefault(s => s.Email == p.email && s.UserName != username);
					if (emailcheck != null) {
						Logger.LoggerFunc("profile/", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
									p, Forbid());
						return Forbid();
					}
					user.Email = p.email;
				}

				if (p.phoneNumber != null) {
					var phonecheck = _context.users.SingleOrDefault(s => s.PhoneNumber == p.phoneNumber && s.UserName != username);
					if (phonecheck != null) {
						Logger.LoggerFunc("profile/", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
									p, Forbid());
						return Forbid();
					}
					user.PhoneNumber = p.phoneNumber;
				}

				//if (!ModelState.IsValid)
				//{
				//	Logger.LoggerFunc("profile/", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
				//				p, BadRequest(ModelState));
				//	return BadRequest(ModelState);
				//}
				//else
				//{
					if(p.firstName!=null) user.FirstName = p.firstName;
					if(p.lastName!=null) user.LastName= p.lastName;
					if(p.profileImage != null) user.ImageUrl= p.profileImage;
					if(p.birthDate != new DateTime()) user.BirthDate = p.birthDate;
					_context.Update(user);
					_context.SaveChanges();
					profileModel p1 = new profileModel()
					{
						id = user.ID,
						username = user.UserName,
						firstName = user.FirstName,
						lastName = user.LastName,
						phoneNumber = user.PhoneNumber,
						email = user.Email,
						profileImage = user.ImageUrl,
						birthDate = user.BirthDate,
						accessLevel = user.AccessLevel,
						restricted = user.Restricted
					};
					Logger.LoggerFunc("profile/", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p, p1);
					return Ok(p1);
				//}
			}
			catch
			{
				Logger.LoggerFunc("profile/", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								p, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpGet]
		[Route("profile/carts")]
		public ActionResult profilecart(int page, int cartsPerPage)
		{
			try
			{
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				if (user == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();

				}

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
					var carts = _context.cart.Where(c => c.CustomerID == customer.ID).ToList();
					var length = carts.Count();

					var totalPages = (int)Math.Ceiling(Convert.ToDecimal(length) / Convert.ToDecimal(cartsPerPage));
					page = Math.Min(totalPages, page);
					var start = Math.Max((page - 1) * cartsPerPage, 0);
					var end = Math.Min(page * cartsPerPage, length);
					var count = Math.Max(end - start, 0);
					cartsPerPage = count;
					carts = carts.GetRange(start, count);


					var carts1 = new List<eachCart>();
					foreach (var c in carts)
					{
						var orders = _context.orders.Where(s => s.CartID == c.ID).ToList();
						var products = new List<eachproduct>();
						foreach (var o in orders)
						{
							var productprice = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);
							eachproduct eachproduct = new eachproduct() { amount = o.Amount, productId = productprice.ProductID };
							products.Add(eachproduct);
						}
						eachCart c1 = new eachCart()
						{
							id = c.ID,
							customerId = c.CustomerID,
							description = c.Description,
							status = c.Status,
							updateDate = c.UpdateDate,
							products = products
						};
						carts1.Add(c1);

					}
					var p = new Pagination<eachCart>()
					{
						page = page,
						perPage = cartsPerPage,
						totalPages = totalPages,
						data = carts1
					};
					return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}




		[HttpGet]
		[Route("profile/carts/{id:Guid}")]
		public ActionResult profilecart(Guid id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				if (username == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();
				}
				var checkcart = _context.cart.SingleOrDefault(c => c.ID == id && c.CustomerID == customer.ID);
				if (checkcart == null)
				{
					return NotFound();
				}
				else
				{
					var orders = _context.orders.Where(s => s.CartID == id).ToList();
					var products = new List<eachproduct>();
					foreach (var o in orders)
					{
						var productprice = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);
						eachproduct eachproduct = new eachproduct() { amount = o.Amount, productId = productprice.ProductID };
						products.Add(eachproduct);
					}
					eachCart c1 = new eachCart()
					{
						id = checkcart.ID,
						customerId = checkcart.CustomerID,
						description = checkcart.Description,
						status = checkcart.Status,
						updateDate = checkcart.UpdateDate,
						products = products
					};

					return Ok(c1);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}


		[HttpGet]
		[Route("profile/subscription")]
		public ActionResult GetProductsubsription(int page, int productsPerPage, Guid productId)
		{
			try
			{

				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				var reqproducts = _context.requestedProducts.Where(c => c.UserID == user.ID).ToList();
				if (username == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();
				}

				if (productId != default(Guid))
				{
					var checkproduct = _context.Products.SingleOrDefault(p => p.ID == productId);
					if (checkproduct == null)
					{
						return NotFound();
					}
					var a = _context.requestedProducts.SingleOrDefault(c => c.ProductID == productId && c.UserID == user.ID);
					var product = _context.Products.SingleOrDefault(p => p.ID == productId);
					if (a == null)
					{
						return NotFound();
					}
					else
					{
						productModel P = new productModel()
						{
							id = product.ID,
							name = product.Name,
							category = product.Category,
							image = product.Image,
							description = product.Description,
							likes = product.likes,
							dislikes = product.dislikes

						};
						return Ok(P);
					}

				}
				else
				{

					if (reqproducts == null)
					{
						return NotFound();
					}
					else
					{
						var length = reqproducts.Count;

						var totalPages = (int)Math.Ceiling((decimal)length / (decimal)productsPerPage);
						page = Math.Min(totalPages, page);
						var start = Math.Max((page - 1) * productsPerPage, 0);
						var end = Math.Min(page * productsPerPage, length);
						var count = Math.Max(end - start, 0);
						productsPerPage = count;

						reqproducts = reqproducts.GetRange(start, count);


						var products = new List<productModel>();
						foreach (var pro in reqproducts)
						{
							var p1 = _context.Products.SingleOrDefault(p => p.ID == pro.ProductID);
							productModel productModel = new productModel()
							{
								id = p1.ID,
								category = p1.Category,
								image = p1.Image,
								description = p1.Description,
								dislikes = p1.dislikes,
								likes = p1.likes,
								name = p1.Name
							};
							products.Add(productModel);
						}
						var p = new Pagination<productModel>()
						{
							page = page,
							totalPages = totalPages,
							perPage = productsPerPage,
							data = products
						};

						return Ok(p);
					}
				}
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpPost]
		[Route("profile/subscribe")]
		public ActionResult GetProductsubsrib(Guid productId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, BadRequest(ModelState));
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				var checkproduct = _context.Products.SingleOrDefault(p => p.ID == productId);
				if (productId == null)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, BadRequest());
					return BadRequest();
				}
				if (username == null)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, Unauthorized());
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, Forbid());
					return Forbid();
				}
				if (checkproduct == null)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, NotFound());
					return NotFound();
				}

				var checksub = _context.requestedProducts.SingleOrDefault(r => r.UserID == user.ID && r.ProductID == productId);
				if (checksub == null)
				{
					RequestedProducts product = new RequestedProducts()
					{
						ID = Guid.NewGuid(),
						ProductID = productId,
						UserID = user.ID
					};
					_context.requestedProducts.Add(product);
					_context.SaveChanges();
					var p = new Dictionary<string, string> { { "status", "success" } };
					// Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
					// productId, p);
					return Ok(p);

				}
				else
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, Ok());
					return Ok();

				}
			}
			catch
			{
				Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpDelete]
		[Route("profile/subscribe")]
		public ActionResult DeleteProductsubsrib(Guid productId)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, BadRequest(ModelState));
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				var checkproduct = _context.Products.SingleOrDefault(p => p.ID == productId);

				if (productId == null)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, BadRequest());
					return BadRequest();
				}
				if (username == null)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, Unauthorized());
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, Forbid());
					return Forbid();
				}
				if (checkproduct == null)
				{
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, NotFound());
					return NotFound();
				}
				var checksub = _context.requestedProducts.SingleOrDefault(r => r.UserID == user.ID && r.ProductID == productId);
				if (checksub == null)
				{
					// Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
					// productId, NotFound());
					return NotFound();
				}
				else
				{
					_context.requestedProducts.Remove(checksub);
					_context.SaveChanges();
					var p = new Dictionary<string, string> { { "status", "success" } };
					Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, p);
					return Ok(p);
				}


			}
			catch
			{
				Logger.LoggerFunc("profile/subscribe", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								productId, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpGet]
		[Route("profile/carts/current")]
		public ActionResult profilecartcurrentget()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				if (username == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();
				}

				var checkcart = _context.cart.SingleOrDefault(c => c.Status.ToLower() == "filling" && c.CustomerID == customer.ID);
				if (checkcart == null)
				{
					return NotFound();
				}
				else
				{
					var orders = _context.orders.Where(s => s.CartID == checkcart.ID).ToList();
					List<eachproduct> eachproducts = new List<eachproduct>();
					foreach (var o in orders)
					{
						var productId = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID).ID;
						eachproduct eachproduct = new eachproduct() { amount = o.Amount, productId = productId };
						eachproducts.Add(eachproduct);

					}
					eachCart p = new eachCart()
					{
						id = checkcart.ID,
						customerId = checkcart.CustomerID,
						description = checkcart.Description,
						products = eachproducts,
						status = checkcart.Status,
						updateDate = checkcart.UpdateDate,
						totalprice = checkcart.TotalPrice
					};
					return Ok(p);

				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}


		[HttpPut]
		[Route("profile/carts/current")]
		public ActionResult profilecartcurrentput([FromBody] eachproduct requestBody)
		{
			//try
			//{
				if (!ModelState.IsValid)
				{
					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								requestBody, BadRequest(ModelState));
					return BadRequest(ModelState);
				}
				else
				{
					var username = User.FindFirstValue(ClaimTypes.Name);
					var user = _context.users.SingleOrDefault(s => s.UserName == username);
					var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
					if (username == null)
					{
						Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								requestBody, Unauthorized());
						return Unauthorized();
					}
					if (user.AccessLevel.ToLower() != "customer")
					{
						Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								requestBody, Forbid());
						return Forbid();
					}

					var checkcart = _context.cart.SingleOrDefault(c => c.Status.ToLower() == "filling" && c.CustomerID == customer.ID);
					if (checkcart == null)
					{
						var cart = new Cart()
						{
							CustomerID = customer.ID,
							ID = Guid.NewGuid(),
							TotalPrice = 0,
							Status = "filling",
							UpdateDate = DateTime.Now,
							Description = ""
						};
						_context.cart.Add(cart);
						_context.SaveChanges();
						checkcart = cart;
					}

					
					var productprice = _context.productPrices.SingleOrDefault(p => p.ID == requestBody.productId);
					if (productprice == null)
					{
						Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
							requestBody, Forbid());
						return NotFound();
					}
					var selectedProduct = _context.Products.SingleOrDefault(p => p.ID == productprice.ProductID);
					if (requestBody.amount > productprice.Amount)
					{
						Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
							requestBody, NotFound());
						return NotFound();
					}

					var checkorder = _context.orders.SingleOrDefault(c => c.CartID == checkcart.ID && c.ProductPriceID == requestBody.productId);

					if (checkorder != null)
					{
						checkorder.Amount = checkorder.Amount + requestBody.amount;

						if(checkorder.Amount <= 0) {
							_context.Remove(checkorder);
							_context.SaveChanges();
						} else {
							_context.Update(checkorder);
							_context.SaveChanges();
						}
					}
					else
					{
						Order order = new Order()
						{
							ID = Guid.NewGuid(),
							Amount = requestBody.amount,
							ProductPriceID = productprice.ID,
							CartID = checkcart.ID
						};
						_context.orders.Add(order);
						_context.SaveChanges();
					}
					double orderprice = 0;
                    var p1 = _context.productPrices.SingleOrDefault(l => l.ID == requestBody.productId);
                    string[] t = p1.Discount.Split(new char[] { '_' });
					
                    if (t.Length == 2 && t[0] == "AMOUNT")
                    {
						double a = Convert.ToDouble(t[1]);
						orderprice = requestBody.amount * Math.Max(productprice.Price - a, 0);
                    }
                    else if (t.Length == 2 && t[0] == "PERCENT")
                    {
						double a = Convert.ToDouble(t[1]);
						orderprice = (requestBody.amount * (productprice.Price * ((100 - a) / 100)));
                    } else {
						orderprice = requestBody.amount * productprice.Price;
					}

                    checkcart.TotalPrice = (checkcart.TotalPrice + orderprice);
					checkcart.UpdateDate = DateTime.Now;
					_context.cart.Update(checkcart);
					_context.SaveChanges();

					var orders = _context.orders.Where(s => s.CartID == checkcart.ID).ToList();
					List<eachproduct> eachproducts = new List<eachproduct>();

					foreach (var o in orders)
					{
						var productPriceId = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID).ID;
						eachproduct eachproduct = new eachproduct() { amount = o.Amount, productId = productPriceId };
						eachproducts.Add(eachproduct);
					}

					eachCart p = new eachCart()
					{
						id = checkcart.ID,
						customerId = checkcart.CustomerID,
						description = checkcart.Description,
						products = eachproducts,
						status = checkcart.Status,
						updateDate = checkcart.UpdateDate,
						totalprice = checkcart.TotalPrice,
					};

					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
							requestBody, p);
					return Ok(p);
				}
			//}
			//catch
			//{
			//	//Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
			//	//				requestBody, StatusCode(StatusCodes.Status500InternalServerError));
			//	return StatusCode(StatusCodes.Status500InternalServerError);
			//}
		}


		[HttpDelete]
		[Route("profile/carts/current")]
		public ActionResult profilecartcurrentdelete()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								null, BadRequest(ModelState));
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				if (username == null)
				{
					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								null, Unauthorized());
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								null, Forbid());
					return Forbid();
				}

				var checkcart = _context.cart.SingleOrDefault(c => c.Status.ToLower() == "filling" && c.CustomerID == customer.ID);
				if (checkcart == null)
				{
					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								null, NotFound());
					return NotFound();
				}

				else
				{   
					var orders = _context.orders.Where(c => c.CartID == checkcart.ID).ToList();
					foreach (var o in orders)
					{
						_context.orders.Remove(o);
						_context.SaveChanges();
					}

					var p = new Dictionary<string, string>() { { "status", "success" } };
					Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								null, p);
					return Ok(p);
				}
			}
			catch
			{
				Logger.LoggerFunc("profile/carts/current", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
								null, StatusCode(StatusCodes.Status500InternalServerError));
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}


		[HttpPut]
		[Route("profile/carts/current/accept")]
		public IActionResult acceptcart()
		{
            var username = User.FindFirstValue(ClaimTypes.Name);
            var user = _context.users.SingleOrDefault(s => s.UserName == username);
			if(user == null)
			{
				return Unauthorized();
			}
            var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
			if (customer == null)
			{
				return Forbid();
			}
				var cart = _context.cart.SingleOrDefault(c => c.CustomerID == customer.ID && c.Status == "Filling");
				if(cart == null)
				{
					return BadRequest();
				}
				else 
				{    if(customer.Balance < cart.TotalPrice)
				  {
                    var b = new Dictionary<string, string>() { { "status", "Rejected" } };
					return Ok(b);

                }
					cart.Status = "Pending";
                var a = new Dictionary<string, string>() { { "status", "Accepted" } };
                cart.UpdateDate = DateTime.Now;
					_context.cart.Update(cart);
					_context.SaveChanges();
					var orders = _context.orders.Where(k => k.CartID == cart.ID).ToList();
					List<eachproduct> eachproducts = new List<eachproduct>();
					foreach(var i in orders)
					{
					if (i.Amount <= 0)
					{
						return BadRequest();
					}
						var productprice = _context.productPrices.SingleOrDefault(p => p.ID == i.ProductPriceID);
						productprice.Amount = productprice.Amount - i.Amount;
					if(productprice.Amount < 0)
					{
						cart.Status = "Rejected";
						cart.UpdateDate = DateTime.Now;
                        var s = new Dictionary<string, string>() { { "status", "Rejected" } };
                        _context.cart.Update(cart);
						_context.SaveChanges();
						return Ok(s);
					}
						
					
						_context.productPrices.Update(productprice);
						_context.SaveChanges();
					}
					
					return Ok(a);
				}

			
            
		}





	}
}