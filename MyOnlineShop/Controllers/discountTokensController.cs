using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Data;
using MyOnlineShop.Models.apimodel;

namespace MyOnlineShop.Controllers
{
	public class discountTokensController : ControllerBase
	{

		private MyShopContext _context;
		public discountTokensController(MyShopContext context)
		{
			_context = context;

		}
		[HttpGet]
		[Route("discountTokens/{id}/Validate")]
		public ActionResult discountTokenGet(Guid id)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var s = new Dictionary<string, string>();
				var token = _context.tokens.Where(t => t.Id == id).Single();

				if (token != null)
				{
					if (DateTime.Now < token.ExpirationDate)
					{
						s = new Dictionary<string, string>() { { "status", "Valid" } };
					}
					else
					{
						s = new Dictionary<string, string>() { { "status", "InValid" } };
					}

				}
				else
				{
					return NotFound();
				}



				return Ok(s);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}



		}





		[HttpPut]
		[Route("discountTokens/{id}/use")]
		[Authorize]
		public ActionResult discountTokenPut(Guid id, Guid cartId)
		{
			try
			{
				Guid userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier).ToString());
				var customer = _context.customer.SingleOrDefault(c => c.UserId == userId);
				var cart = _context.cart.SingleOrDefault(C => C.ID == cartId);
				var token1 = _context.tokens.Where(t => t.Id == id).Single();
				if (customer == null)
				{
					return Unauthorized();
				}
				if (cart == null)
				{
					return NotFound();
				}
				if (cart.Status == "Filling" || cart.TotalPrice == 0)
				{
					return BadRequest();
				}
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				var status = "Invalid";
				if (token1 != null && DateTime.Now <= token1.ExpirationDate)

				{
					status = "Valid";
					string[] t = token1.Discount.Split(new char[] { '_' });
					double a = Convert.ToDouble(t[1]);
					if (t[0] == "AMOUNT")
					{
						cart.TotalPrice = cart.TotalPrice - a;
					}
					else if (t[0] == "PERCENT")
					{
						cart.TotalPrice = cart.TotalPrice - (cart.TotalPrice * a);
					}
					_context.Update(cart);

					_context.SaveChanges();
				}
				var orders = _context.orders.Where(o => o.CartID == cartId).ToList();
				var ps = new List<eachproduct>();
				foreach (var o in orders)
				{
					var product = _context.productPrices.SingleOrDefault(p => p.ID == o.ProductPriceID);

					eachproduct p = new eachproduct()
					{
						productId = product.ProductID,
						amount = o.Amount

					};
					ps.Add(p);
				}
				eachCart eachCart = new eachCart()
				{
					customerId = cart.CustomerID,
					description = cart.Description,
					id = cart.ID,
					products = ps,
					status = "Filling",
					updateDate = cart.UpdateDate


				};
				token t1 = new token()
				{
					status = status,
					cart = eachCart
				};

				return Ok(t1);


			}

			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}




		}
	}
}

