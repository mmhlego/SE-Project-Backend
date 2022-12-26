using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


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
