using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;
using Newtonsoft.Json;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using MyOnlineShop.Data;
using System.Web.Http.Filters;

namespace MyOnlineShop.Controllers
{
	[Authorize(Roles = "admin,Admin,Administrator")]
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
						restricted = u.Restricted
					})
					.ToList()
			};

			return Ok(users);
		}

		//------------------------------------

		[HttpGet]
		[Route("admin/users/{id}")]
		public ActionResult eachuserget(Guid id)
		{

				try
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
						restricted = userId.Restricted
					};
					return Ok(user);
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
                User userput = new User();
                var userId = _context.users.SingleOrDefault(p => p.ID == id);

                if (userId == null)
                {
                    return StatusCode(StatusCodes.Status404NotFound);
                }
                else
                {
                    userput = new User()
                    {
                        ID = userId.ID,
						UserName = userId.UserName,
                        FirstName = userId.FirstName,
                        LastName = userId.LastName,
                        ImageUrl = userId.ImageUrl,
                        Password = userId.Password,
						BirthDate = userId.BirthDate,
						customers = userId.customers,
						sellers = userId.sellers,
						PhoneNumber = req.phoneNumber,
						Email = req.email,
						AccessLevel = req.accessLevel,
						Restricted = req.restricted
                    };
                    _context.users.Add(userput);
                    _context.SaveChanges();
                    return Ok(userput);
                }

                if (!ModelState.IsValid)
                {
                    return StatusCode(StatusCodes.Status400BadRequest);
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
					User userput = new User();
					var userId = _context.users.SingleOrDefault(p => p.ID == id);
					bool restricted = true;
					if (userId == null)
					{
						return StatusCode(StatusCodes.Status404NotFound);
					}
					else
					{
						userput = new User()
						{
							ID = userId.ID,
							UserName = userId.UserName,
							FirstName = userId.FirstName,
							LastName = userId.LastName,
							ImageUrl = userId.ImageUrl,
							Password = userId.Password,
							BirthDate = userId.BirthDate,
							customers = userId.customers,
							sellers = userId.sellers,
							PhoneNumber = userId.PhoneNumber,
							Email = userId.Email,
							AccessLevel = userId.AccessLevel,
							Restricted = restricted
						};
						_context.users.Add(userput);
						_context.SaveChanges();
						return Ok(userput);
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
					DiscountToken tokenput = new DiscountToken()
					{
						ExpirationDate = tokenputter.ExpirationDate,
						Discount = tokenputter.Discount,
						IsEvent = tokenputter.IsEvent
					};

                        
                    _context.discountTokens.Add(tokenput);
                    _context.SaveChanges();
                    return Ok(tokenput);

                    if (!ModelState.IsValid)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest);
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
					var delToken = _context.discountTokens.SingleOrDefault(p => p.Id == id);
					if(delToken == null)
					{
						return StatusCode(StatusCodes.Status404NotFound);
					}
					else
					{
						_context.discountTokens.Remove(delToken);
						_context.SaveChanges();
						
					}
                    return Ok();
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
                    var cartId = _context.cart.SingleOrDefault(p => p.ID == id);

                    if (cartId == null)
                    {
                        return StatusCode(StatusCodes.Status404NotFound);
                    }
                    else
                    {
                        Cart cartput = new Cart()
                        {
                            ID = cartId.ID,
                            CustomerID = cartId.CustomerID,
                            Products = cartId.Products,
                            Status = status,
                            Discription = cartId.Discription,
                            UpdateDate = cartId.UpdateDate
                        };
						_context.cart.Add(cartput);
						_context.SaveChanges();
                        return Ok(cartput);
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
				catch
				{
					return StatusCode(StatusCodes.Status500InternalServerError);
				}
		}
	}
}



