using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models;
using System.Security.Claims;
using MyOnlineShop.Services;
using MyOnlineShop.Data;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using MyOnlineShop.Models.apimodel;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;

namespace MyOnlineShop.Controllers
{
	public class SellerController : ControllerBase
	{


		private MyShopContext _context;
		public SellerController(MyShopContext context)
		{
			_context = context;

		}

		[HttpGet]
		[Route("sellers")]
		public IActionResult GetAllSellers(int SellersPerPage, int page)
		{
			if (!ModelState.IsValid)
			{
				return StatusCode(StatusCodes.Status400BadRequest);
			}
			try
			{

				List<Seller> seller = _context.sellers.ToList();
				List<Seller> sellers = new List<Seller>();

				if (seller != null)
				{

					if ((page * SellersPerPage) - SellersPerPage < seller.Count)
					{
						if (page * SellersPerPage > seller.Count)
						{
							sellers = seller.GetRange((page * SellersPerPage) - SellersPerPage, seller.Count);

						}
						else
						{
							sellers = seller.GetRange((page * SellersPerPage) - SellersPerPage, seller.Count);

						}

					}
					else
					{
						return StatusCode(StatusCodes.Status400BadRequest);
					}
				}


				else
				{
					return StatusCode(StatusCodes.Status404NotFound);

				}
				List<SellerSchema> sellerSchema = new List<SellerSchema>();

				foreach (var ss in seller)
				{
					var user = _context.users.SingleOrDefault(p => p.ID == ss.UserId);
					SellerSchema schema = new SellerSchema()
					{
						information = ss.Information,
						address = ss.Address,
						id = ss.ID,
						dislikes = ss.dislikes,
						likes = ss.likes,
						image = user.ImageUrl,
						name = user.FirstName + " " + user.LastName,
						restricted = user.Restricted
					};
					sellerSchema.Add(schema);
				}

				if (seller == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				if (!ModelState.IsValid)
				{
					return StatusCode(StatusCodes.Status400BadRequest);
				}
				SellersSchema s = new SellersSchema()
				{
					sellersPerPage = SellersPerPage,
					page = page,
					sellers = sellerSchema

				};

				return Ok(s);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}




		[HttpGet]
		[Route("sellers/{sellerId:Guid}")]
		public ActionResult GetSeller(Guid sellerId)
		{
			try
			{
				var ss = _context.sellers.SingleOrDefault((p) => p.ID == sellerId);
				var user = _context.users.SingleOrDefault(p => p.ID == ss.UserId);
				SellerSchema schema = new SellerSchema()
				{
					information = ss.Information,
					address = ss.Address,
					id = ss.ID,
					dislikes = ss.dislikes,
					likes = ss.likes,
					image = user.ImageUrl,
					name = user.UserName,
					restricted = user.Restricted

				};
				if (ss == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				if (!ModelState.IsValid)
				{
					return StatusCode(StatusCodes.Status400BadRequest);
				}
				return Ok(schema);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}

		[HttpPut]
		[Route("sellers/{sellerId:Guid}")]
		public ActionResult UpdateSeller(Guid sellerId, [FromBody] SellerpagePutMethodRequest s)
		{
			try
			{
				Seller schema = new Seller();
				var ss = _context.sellers.SingleOrDefault((p) => p.UserId == sellerId);

				if (ss == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				else
				{
					var user = _context.users.SingleOrDefault(p => p.ID == ss.UserId);
					schema = new Seller()
					{
						Information = s.information,
						Address = s.address,
						UserId = ss.UserId,
						dislikes = ss.dislikes,
						likes = ss.likes,
						user = user
					};
					_context.sellers.Add(schema);
					Logger.LoggerFunc($"sellers/{sellerId:Guid}",
							_context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, schema);
					_context.SaveChanges();
				}

				if (!ModelState.IsValid)
				{
					return StatusCode(StatusCodes.Status400BadRequest);
				}
				return Ok(schema);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}



		[HttpGet]
		[Route("sellers/{id}/stats")]
		public ActionResult<IEnumerable<statsModel>> SelleresStatsId(Guid id, statsReqModel s, [FromQuery] int page, [FromQuery] int statsPerPage)
		{

			try
			{
				var stats = new statsModel
				{
					page = page,
					allstatsPerPage = statsPerPage,
					stats = _context.stats.Where(d => d.productId == s.productId && d.sellerId == id && d.date >= s.datefrom && d.date <= s.dateto)
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


		[HttpPut]
		[Route("Sellers/{id}/likes")]
		public ActionResult<IEnumerable<SellerSchema>> SellerLikes(Guid id, bool like)
		{
			try
			{
				Seller seller = new Seller();
				var SellerId = _context.sellers.SingleOrDefault(l => l.UserId == id);

				if (SellerId == null)
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}
				else
				{
					var user = _context.users.SingleOrDefault(p => p.ID == SellerId.UserId);
					if (like)
					{
						seller = new Seller()
						{
							likes = seller.likes + 1,
							Address = SellerId.Address,
							Information = SellerId.Information,
							dislikes = SellerId.dislikes,
							UserId = SellerId.UserId,
							user = user
						};
					}
					else
					{
						seller = new Seller()
						{
							likes = seller.likes,
							Address = SellerId.Address,
							Information = SellerId.Information,
							dislikes = SellerId.dislikes + 1,
							UserId = SellerId.UserId,
							user = user
						};
					}
					_context.sellers.Add(seller);
					Logger.LoggerFunc($"Sellers/{id}/likes",
							_context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID, seller);
					_context.SaveChanges();
				}
				return Ok(seller);
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
