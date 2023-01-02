using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using MyOnlineShop.Data;
using System.Web.Http.Filters;
using System.Security.Claims;

namespace MyOnlineShop.Controllers
{
	public class AdminController : ControllerBase
	{
		private readonly MyShopContext _context;

		public AdminController(MyShopContext context)
		{
			_context = context;
		}

		//------------------------------------
		[HttpGet]
		[Route("admin/users")]
		public ActionResult<IEnumerable<usersModel>> userssget([FromQuery] int page, [FromQuery] int usersPerPage)
		{
			try
			{
				string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
				if (accessLevel == "admin")
				{
					var users = new usersModel
					{
						page = page,
						usersPerPage = usersPerPage,
						users = _context.users
							.Skip((page - 1) * usersPerPage)
							.Take(usersPerPage)
							.Select(u => new userModel
							{
								id = u.ID,
								username = u.UserName,
								firstName = u.FirstName,
								lastName = u.LastName,
								phoneNumber = u.PhoneNumber,
								email = u.Email,
								profileImage = u.ImageUrl,
								birthDate = u.BirthDate,
								accessLevel = u.AccessLevel,
								isApproved = u.IsApproved,
								restricted = u.Restricted
							})
							.ToList()
					};

					return Ok(users);
				}
				else
				{
					return StatusCode(StatusCodes.Status403Forbidden);
				}
			}
			catch
			{
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

		//------------------------------------

		[HttpGet]
		[Route("admin/users/{id}")]
		public ActionResult eachuserget(Guid id)
		{
			try { 
				string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
				if (accessLevel == "admin")
				{
					var userId = _context.users.SingleOrDefault(p => p.ID == id);
					var user = new userModel
					{
						id = userId.ID,
						username = userId.UserName,
						firstName = userId.FirstName,
						lastName = userId.LastName,
						phoneNumber = userId.PhoneNumber,
						email = userId.Email,
						profileImage = userId.ImageUrl,
						birthDate = userId.BirthDate,
						accessLevel = userId.AccessLevel,
						isApproved = userId.IsApproved,
						restricted = userId.Restricted
					};
					return Ok(user);
				}
                else
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }
            }
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}



		[HttpPut]
		[Route("admin/users/{id}")]
		public ActionResult userput(Guid id, [FromBody] userreqModel req)
		{
            try
            {
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
				if (accessLevel == "admin")
				{
					//User userput = new User();
					var userId = _context.users.SingleOrDefault(p => p.ID == id);

					if (userId == null)
					{
						return StatusCode(StatusCodes.Status404NotFound);
					}
					else
					{
						userId.PhoneNumber = req.phoneNumber;
						userId.Email = req.email;
						userId.AccessLevel = req.accessLevel;
						userId.Restricted = req.restricted;
						_context.SaveChanges();
						return Ok(userId);
					}

					if (!ModelState.IsValid)
					{
						return StatusCode(StatusCodes.Status400BadRequest);
					}
				}
				else
				{
					return StatusCode(StatusCodes.Status403Forbidden);
				}
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }




		[HttpDelete]
		[Route("admin/users/{id}")]
		public ActionResult userdelete(Guid id)
		{
				try
				{
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						//User userDelete = new User();
						var userId = _context.users.SingleOrDefault(p => p.ID == id);
						bool restricted = true;
						if (userId == null)
						{
							return StatusCode(StatusCodes.Status404NotFound);
						}
						else
						{
							userId.Restricted = restricted;
							_context.SaveChanges();
							return Ok(userId);
						}
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
				}
				catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}



		[HttpGet]
		[Route("admin/discountTokens")]
		public ActionResult<IEnumerable<tokensModel>> discountTokens(bool isEvent, bool expired, [FromQuery] int page, [FromQuery] int tokensPerPage)
		{
				try
				{
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						var tokens = new tokensModel();
						if (expired)
						{
							tokens = new tokensModel
							{
								page = page,
								tokensPerPage = tokensPerPage,
								tokens = _context.tokens.Where(d => d.ExpirationDate < DateTime.Now && d.IsEvent == isEvent)
														.Skip((page - 1) * tokensPerPage)
														.Take(tokensPerPage)
														.Select(u => new DiscountToken
														{
															Id = u.Id,
															ExpirationDate = u.ExpirationDate,
															Discount = u.Discount,
															IsEvent = u.IsEvent
														}).ToList()

							};
							if (tokens.tokens.Count == 0)
								return StatusCode(StatusCodes.Status400BadRequest);

							return Ok(tokens);
						}
						else
						{
							tokens = new tokensModel
							{
								page = page,
								tokensPerPage = tokensPerPage,
								tokens = _context.tokens.Where(d => d.ExpirationDate >= DateTime.Now && d.IsEvent == isEvent)
														.Skip((page - 1) * tokensPerPage)
														.Take(tokensPerPage)
														.Select(u => new DiscountToken
														{
															Id = u.Id,
															ExpirationDate = u.ExpirationDate,
															Discount = u.Discount,
															IsEvent = u.IsEvent
														}).ToList()

							};
							if (tokens.tokens.Count == 0)
								return StatusCode(StatusCodes.Status400BadRequest);

							return Ok(tokens);
						}
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
				}
				catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}



		[HttpPost]
		[Route("admin/discountTokens")]
		public ActionResult discountTokenspost([FromBody] tokenreqModel tokenputter)
		{
				try
				{
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						DiscountToken tokenput = new DiscountToken()
						{
							ExpirationDate = tokenputter.ExpirationDate,
							Discount = tokenputter.Discount,
							IsEvent = tokenputter.IsEvent
						};


						_context.tokens.Add(tokenput);
						_context.SaveChanges();
						return Ok(tokenput);

						if (!ModelState.IsValid)
						{
							return StatusCode(StatusCodes.Status400BadRequest);
						}
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
                }
				catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}




		[HttpDelete]
		[Route("admin/discountTokens/{id}")]
		public ActionResult discountTokensdelete(Guid id)
		{
				try
				{
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						var delToken = _context.tokens.SingleOrDefault(p => p.Id == id);
						if (delToken == null)
						{
							return StatusCode(StatusCodes.Status404NotFound);
						}
						else
						{
							_context.tokens.Remove(delToken);
							_context.SaveChanges();

						}
						return Ok();
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
                }
                catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}

		[HttpGet]
		[Route("admin/carts")]
		public ActionResult<IEnumerable<cartModel>> admincarts(Guid? userId, bool? current, [FromQuery] int page, [FromQuery] int cartsPerPage)
		{
			try
			{
                string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
				if (accessLevel == "admin")
				{
					var carts = new cartModel();
					if (current.HasValue)
					{
						if (current.Value == true)
						{
							//current = True => status = Approved
							if (userId.HasValue)
							{
								carts = new cartModel
								{
									page = page,
									cartsPerPage = cartsPerPage,
									carts = _context.cart.Where(d => d.Status == "Approved" && d.ID == userId)
									.Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
									{
										id = u.ID,
										customerId = u.CustomerID,
										products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
										{
											productId = l.ProductID,
											amount = l.Amount
										}).ToList(),
										status = u.Status,
										description = u.Discription,
										updateDate = u.UpdateDate
									}).ToList()
								};
							}
							else
							{
								carts = new cartModel
								{
									page = page,
									cartsPerPage = cartsPerPage,
									carts = _context.cart.Where(d => d.Status == "Approved")
									.Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
									{
										id = u.ID,
										customerId = u.CustomerID,
										products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
										{
											productId = l.ProductID,
											amount = l.Amount
										}).ToList(),
										status = u.Status,
										description = u.Discription,
										updateDate = u.UpdateDate
									}).ToList()
								};

							}
						}
						else if (current.Value == false)
						{
							//current = fasle => status = Rejected
							if (userId.HasValue)
							{
								carts = new cartModel
								{
									page = page,
									cartsPerPage = cartsPerPage,
									carts = _context.cart.Where(d => d.Status == "Rejected" && d.ID == userId)
									.Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
									{
										id = u.ID,
										customerId = u.CustomerID,
										products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
										{
											productId = l.ProductID,
											amount = l.Amount
										}).ToList(),
										status = u.Status,
										description = u.Discription,
										updateDate = u.UpdateDate
									}).ToList()
								};
							}
							else
							{
								carts = new cartModel
								{
									page = page,
									cartsPerPage = cartsPerPage,
									carts = _context.cart.Where(d => d.Status == "Rejected")
									.Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
									{
										id = u.ID,
										customerId = u.CustomerID,
										products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
										{
											productId = l.ProductID,
											amount = l.Amount
										}).ToList(),
										status = u.Status,
										description = u.Discription,
										updateDate = u.UpdateDate
									}).ToList()
								};

							}
						}
					}
					else
					{
						if (userId.HasValue)
						{
							carts = new cartModel
							{
								page = page,
								cartsPerPage = cartsPerPage,
								carts = _context.cart.Where(d => d.ID == userId)
								.Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
								{
									id = u.ID,
									customerId = u.CustomerID,
									products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
									{
										productId = l.ProductID,
										amount = l.Amount
									}).ToList(),
									status = u.Status,
									description = u.Discription,
									updateDate = u.UpdateDate
								}).ToList()
							};
						}
						else
						{
							carts = new cartModel
							{
								page = page,
								cartsPerPage = cartsPerPage,
								carts = _context.cart
								.Skip((page - 1) * cartsPerPage).Take(cartsPerPage).Select(u => new eachCart
								{
									id = u.ID,
									customerId = u.CustomerID,
									products = _context.productPrices.Where(f => f.SellerID == userId).Select(l => new eachproduct
									{
										productId = l.ProductID,
										amount = l.Amount
									}).ToList(),
									status = u.Status,
									description = u.Discription,
									updateDate = u.UpdateDate
								}).ToList()
							};

						}
					}
					return Ok(carts);
				}
				else
				{
					return StatusCode(StatusCodes.Status403Forbidden);
				}
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}




		[HttpGet]
		[Route("admin/carts/{id:Guid}")]
		public ActionResult admincart(Guid id)
		{
				try
				{
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						var cartId = _context.cart.SingleOrDefault(p => p.ID == id);

						if (cartId == null)
						{
							return StatusCode(StatusCodes.Status404NotFound);
						}
						else
						{
							eachCart cart = new eachCart()
							{
								id = cartId.ID,
								customerId = cartId.CustomerID,
								products = _context.productPrices.Where(f => f.SellerID == id).Select(l => new eachproduct
								{
									productId = l.ProductID,
									amount = l.Amount
								}).ToList(),
								status = cartId.Status,
								description = cartId.Discription,
								updateDate = cartId.UpdateDate
							};
							return Ok(cart);
						}
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
				}
				catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}





		[HttpPut]
		[Route("admin/carts/{id}")]
		public ActionResult admincartsput(Guid id, string status)
		{
                try
                {
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						var cartId = _context.cart.SingleOrDefault(p => p.ID == id);

						if (cartId == null)
						{
							return StatusCode(StatusCodes.Status404NotFound);
						}
						else
						{
							cartId.Status = status;
							_context.SaveChanges();
							return Ok(cartId);
						}
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
                }
                catch
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
		}



		[HttpGet]
		[Route("admin/stats")]
		public ActionResult<IEnumerable<statsModel>> sellerstate(Guid sellerId, statsReqModel s, [FromQuery] int page, [FromQuery] int statsPerPage)
		{
				try
				{
					string accessLevel = User.FindFirstValue(ClaimTypes.Role).ToLower();
					if (accessLevel == "admin")
					{
						var stats = new statsModel
						{
							page = page,
							allstatsPerPage = statsPerPage,
							stats = _context.stats.Where(d => d.productId == s.productId && d.sellerId == sellerId && d.date >= s.datefrom && d.date <= s.dateto)
							.Skip((page - 1) * statsPerPage)
							.Take(statsPerPage)
							.Select(u => new statModel
							{
								id = u.Id,
								productId = u.productId,
								sellerId = u.sellerId,
								date = u.date,
								amount = u.amount,
								price = u.price
							}).ToList()

						};
						if (stats.stats.Count == 0)
							return StatusCode(StatusCodes.Status400BadRequest);

						return Ok(stats);
					}
					else
					{
						return StatusCode(StatusCodes.Status403Forbidden);
					}
				}
				catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}
	}
}



