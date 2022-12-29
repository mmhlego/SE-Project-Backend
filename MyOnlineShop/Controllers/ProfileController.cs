using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models.apimodel;

namespace MyOnlineShop.Controllers
{
	public class ProfileController : ControllerBase
	{
		[HttpGet]
		[Route("profile/")]
		public ActionResult GetProfiles()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
					profileModel p = new profileModel();
					return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}



		[HttpPut]
		[Route("profile/")]
		public ActionResult putProfile(putprofileModel putprofileModel)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
					profileModel p = new profileModel();
					return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}



		[HttpGet]
		[Route("profile/carts")]
		public ActionResult profilecart()
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}
				else
				{
					cartModel p = new cartModel();
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
				else
				{
					eachCart p = new eachCart();
					return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}


		[HttpGet]
		[Route("profile/subscription")]
		public ActionResult GetProductsubsription(Guid id)
		{
			try
			{
				productsModel p = new productsModel();
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				return Ok(p);
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
				var p = new Dictionary<string, string> { { "status", "success" } };
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				return Ok(p);
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
				var p = new Dictionary<string, string> { { "status", "success" } };
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				return Ok(p);
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
				else
				{
					cartModel p = new cartModel();
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
					cartModel p = new cartModel();
					return Ok(p);
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
				else
				{
					var p = new Dictionary<string, string>() { { "status", "success" } };
					return Ok(p);
				}
			}
			catch { return StatusCode(StatusCodes.Status500InternalServerError); }
		}


	}
}

