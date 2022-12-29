using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models;

namespace MyOnlineShop.Controllers
{
	public class RequestedProductsController : ControllerBase
	{
		RequestedProducts[] requestedproducts = new RequestedProducts[999];

		public IEnumerable<RequestedProducts> GetAllRequestedProducts()
		{
			// Get: RequestedProducts
			return requestedproducts;
		}
		public ActionResult GetRequestedProduct(Guid id)
		{
			var requestedproduct = requestedproducts.FirstOrDefault((p) => p.ID == id);
			if (requestedproduct == null)
			{
				return NotFound();
			}
			return Ok(requestedproduct);
		}
	}
}
