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
using NuGet.Protocol.Plugins;

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
				if (page == 0)
				{
					page = 1;
				}
				if (SellersPerPage == 0)
				{
					SellersPerPage = 5;
				}

				List<Seller> seller = _context.sellers.ToList();
				List<Seller> sellers = new List<Seller>();

				if (seller != null)
				{


					if (page * SellersPerPage > seller.Count)
					{
						sellers = seller.GetRange((page * SellersPerPage) - SellersPerPage, seller.Count);

					}
					else
					{
						sellers = seller.GetRange((page * SellersPerPage) - SellersPerPage, SellersPerPage);

					}
				}
				else
				{
					return StatusCode(StatusCodes.Status404NotFound);

				}
				List<SellerSchema> sellerSchema = new List<SellerSchema>();

				foreach (var ss in sellers)
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
				if (!ModelState.IsValid)
				{
					return StatusCode(StatusCodes.Status400BadRequest);
				}
				var s = _context.sellers.ToList();
				Seller ss = null;
				int i = 0;
				foreach (var t in s)
				{
					if (t.ID == sellerId)
					{
						ss = s.Single(x => x.ID == sellerId);
						i++;
					}
				}


				if (i > 0)
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
						name = user.UserName,
						restricted = user.Restricted

					};
					return Ok(schema);

				}
				else
				{
					return StatusCode(StatusCodes.Status404NotFound);
				}


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
				SellerSchema schema = new SellerSchema();

				var s1 = _context.sellers.ToList();
				Seller ss = null;
				int i = 0;
				foreach (var t in s1)
				{
					if (t.ID == sellerId)
					{
						ss = s1.Single(x => x.ID == sellerId);
						i++;
					}
				}

				if (ss == null)
				{
                    Logger.LoggerFunc($"sellers/{sellerId:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                s, StatusCode(StatusCodes.Status404NotFound));
                    return StatusCode(StatusCodes.Status404NotFound);
				}
				else
				{
					ss.Address = s.address;
					ss.Information = s.information;
					_context.sellers.UpdateRange();
					_context.SaveChanges();
					var user = _context.users.SingleOrDefault(p => p.ID == ss.UserId);
					schema = new SellerSchema()
					{
						id = ss.ID,
						name = user.FirstName + " " + user.LastName,
						image = user.ImageUrl,
						information = s.information,
						address = s.address,
						dislikes = ss.dislikes,
						likes = ss.likes,
						restricted = user.Restricted
					};
				}
				if (!ModelState.IsValid)
				{
                    Logger.LoggerFunc($"sellers/{sellerId:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                s, StatusCode(StatusCodes.Status400BadRequest));
                    return StatusCode(StatusCodes.Status400BadRequest);
				}
                Logger.LoggerFunc($"sellers/{sellerId:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                s, schema);
                return Ok(schema);
			}
			catch
			{
                Logger.LoggerFunc($"sellers/{sellerId:Guid}", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                s, StatusCode(StatusCodes.Status500InternalServerError));
                return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}



		[HttpGet]
		[Route("sellers/{id}/stats")]
		public ActionResult<IEnumerable<statsModel>> SelleresStatsId(Guid id, statsReqModel s, [FromQuery] int page, [FromQuery] int statsPerPage)
		{

			try
			{
				var stats = _context.stats.Where(s => s.sellerId == id).ToList();

				if (page == 0)
				{
					page = 1;
				}
				if (statsPerPage == 0)
				{
					statsPerPage = 10;
				}

				if (s.productId != default(Guid))
				{
					stats = stats.Where(c => c.productId == s.productId).ToList();
				}
				if (s.datefrom != default(DateTime))
				{
					stats = stats.Where(c => c.date >= s.datefrom).ToList();
				}
				if (s.dateto != default(DateTime))
				{
					stats = stats.Where(c => c.date <= s.dateto).ToList();
				}

				List<Stats> statsForShow;

				if ((page * statsPerPage) - statsPerPage < stats.Count)
				{
					if (page * statsPerPage > stats.Count)
					{
						statsForShow = stats.GetRange((page * statsPerPage) - statsPerPage, stats.Count);

					}
					else
					{
						statsForShow = stats.GetRange((page * statsPerPage) - statsPerPage, page * statsPerPage);

					}
				}
				else
				{
					return NotFound();
				}
				List<statModel> lastList = new List<statModel>();
				foreach (var last in statsForShow)
				{
					statModel stForAdd = new statModel()
					{
						productId = last.productId,
						sellerId = last.sellerId,
						date = last.date,
						amount = last.amount,
						price = last.price
					};
					lastList.Add(stForAdd);
				}
				var toshow = new statsModel()
				{
					page = page,
					allstatsPerPage = statsPerPage,
					stats = lastList
				};
				return Ok(toshow);
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
				var s1 = _context.sellers.ToList();
				Seller ss = null;
				int i = 0;
				foreach (var t in s1)
				{
					if (t.ID == id)
					{
						ss = s1.Single(x => x.ID == id);
						i++;
					}
				}

				if (ss == null)
				{
                    Logger.LoggerFunc($"Sellers/{id}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                like, StatusCode(StatusCodes.Status404NotFound));
                    return StatusCode(StatusCodes.Status404NotFound);
				}
				else
				{
					var user = _context.users.SingleOrDefault(p => p.ID == ss.UserId);
					var seller = new SellerSchema();
					if (like)
					{
						ss.likes += 1;
						_context.sellers.UpdateRange();
						_context.SaveChanges();
						seller = new SellerSchema()
						{
							id = ss.ID,
							name = user.FirstName + " " + user.LastName,
							image = user.ImageUrl,
							information = ss.Information,
							address = ss.Address,
							dislikes = ss.dislikes,
							likes = ss.likes,
							restricted = user.Restricted
						};
                        Logger.LoggerFunc($"Sellers/{id}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                like, seller);
                        return Ok(seller);
					}
					else
					{
						ss.dislikes += 1;
						_context.sellers.UpdateRange();
						_context.SaveChanges();
						seller = new SellerSchema()
						{
							id = ss.ID,
							name = user.FirstName + " " + user.LastName,
							image = user.ImageUrl,
							information = ss.Information,
							address = ss.Address,
							dislikes = ss.dislikes,
							likes = ss.likes,
							restricted = user.Restricted
						};
                        Logger.LoggerFunc($"Sellers/{id}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                like, seller);
                        return Ok(seller);
					}
				}
			}
			catch
			{
                Logger.LoggerFunc($"Sellers/{id}/likes", _context.users.FirstOrDefault(l => l.UserName == User.FindFirstValue(ClaimTypes.Name)).ID,
                                like, StatusCode(StatusCodes.Status500InternalServerError));
                return StatusCode(StatusCodes.Status500InternalServerError);
			}
		}
	}
}
