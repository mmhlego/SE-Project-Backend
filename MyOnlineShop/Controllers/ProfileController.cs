using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Services;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;

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
		public ActionResult putProfile(putprofileModel p)
		{
			try
			{
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);

				if (user == null)
				{
					return Unauthorized();
				}
				var emailcheck = _context.users.SingleOrDefault(s => s.Email == p.email && s.UserName != username);
				var phonecheck = _context.users.SingleOrDefault(s => s.PhoneNumber == p.phoneNumber && s.UserName != username);
				if (emailcheck != null || phonecheck != null)
				{
					return Forbid();
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
					user.FirstName = p.firstName;
					user.LastName = p.lastName;
					user.PhoneNumber = p.phoneNumber;
					user.Email = p.email;
					user.ImageUrl = p.profileImage;
					user.BirthDate = p.birthDate;
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
                    Logger.LoggerFunc(DateTime.Now, "profile",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, p1);
                    return Ok(p1);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
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
					if ((page * cartsPerPage) - cartsPerPage < carts.Count)
					{
						if (page * cartsPerPage > carts.Count)
						{
							carts = carts.GetRange((page * cartsPerPage) - cartsPerPage, carts.Count);
						}
						else
						{
							carts = carts.GetRange((page * cartsPerPage) - cartsPerPage, cartsPerPage);
						}
					}
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
					cartModel p = new cartModel()
					{
						page = page,
						cartsPerPage = cartsPerPage,
						carts = carts1
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
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				var checkproduct = _context.Products.SingleOrDefault(p => p.ID == productId);
				if (username == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();
				}

				if (productId != null)
				{
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
					var reqproducts = _context.requestedProducts.Where(c => c.UserID == user.ID).ToList();
					if (reqproducts == null)
					{
						return NotFound();
					}
					else
					{
						if ((page * productsPerPage) - productsPerPage < reqproducts.Count)
						{
							if (page * productsPerPage > reqproducts.Count)
							{
								reqproducts = reqproducts.GetRange((page * productsPerPage) - productsPerPage, reqproducts.Count);
							}
							else
							{
								reqproducts = reqproducts.GetRange((page * productsPerPage) - productsPerPage, productsPerPage);
							}
						}
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
						productsModel p = new productsModel()
						{
							page = page,
							productsPerPage = productsPerPage,
							products = products
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
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				var checkproduct = _context.Products.SingleOrDefault(p => p.ID == productId);
				if (productId == null)
				{
					return BadRequest();
				}
				if (username == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();
				}
				if (checkproduct == null)
				{
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
                    Logger.LoggerFunc(DateTime.Now, "profile/subscribe",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, p);
                    return Ok(p);

				}
				else
				{
					return Ok();

				}
			}
			catch
			{
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
					return BadRequest(ModelState);
				}
				var username = User.FindFirstValue(ClaimTypes.Name);
				var user = _context.users.SingleOrDefault(s => s.UserName == username);
				var customer = _context.customer.SingleOrDefault(c => c.UserId == user.ID);
				var checkproduct = _context.Products.SingleOrDefault(p => p.ID == productId);

				if (productId == null)
				{
					return BadRequest();
				}
				if (username == null)
				{
					return Unauthorized();
				}
				if (user.AccessLevel.ToLower() != "customer")
				{
					return Forbid();
				}
				if (checkproduct == null)
				{
					return NotFound();
				}
				var checksub = _context.requestedProducts.SingleOrDefault(r => r.UserID == user.ID && r.ProductID == productId);
				if (checksub == null)
				{
					return NotFound();
				}
				else
				{
					_context.requestedProducts.Remove(checksub);
					_context.SaveChanges();
					var p = new Dictionary<string, string> { { "status", "success" } };
                    Logger.LoggerFunc(DateTime.Now, "profile/subscribe",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, p);
                    return Ok(p);
				}


			}
			catch
			{
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
						var productId = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID).ProductID;
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
						updateDate = checkcart.UpdateDate
					};
					return Ok(p);

				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}




		[HttpPut]
		[Route("profile/carts/current")]
		public ActionResult profilecartcurrentput([FromBody] eachproduct product)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
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
						var productprice = _context.productPrices.SingleOrDefault(p => p.ProductID == product.productId);
						if (productprice == null)
						{
							return NotFound();
						}

						Order order = new Order()
						{
							ID = Guid.NewGuid(),
							Amount = product.amount,
							ProductPriceID = productprice.ID,
							CartID = checkcart.ID

						};
						_context.orders.Add(order);
						_context.SaveChanges();
						var orders = _context.orders.Where(s => s.CartID == checkcart.ID).ToList();
						List<eachproduct> eachproducts = new List<eachproduct>();
						foreach (var o in orders)
						{
							var productId = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID).ProductID;
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
							updateDate = checkcart.UpdateDate
						};
                        Logger.LoggerFunc(DateTime.Now, "profile/carts/current",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, p);
                        return Ok(p);

					}
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}




		[HttpDelete]
		[Route("profile/carts/current")]
		public ActionResult profilecartcurrentdelete()
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
					var orders = _context.orders.Where(c => c.CartID == checkcart.ID).ToList();
					foreach (var o in orders)
					{
						_context.orders.Remove(o);
						_context.SaveChanges();
					}

					var p = new Dictionary<string, string>() { { "status", "success" } };
                    Logger.LoggerFunc(DateTime.Now, "profile/carts/current",
                            _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, p);
                    return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}


	}
}

