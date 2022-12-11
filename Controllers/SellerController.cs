using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyOnlineShop.Data;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using HttpPostAttribute = Microsoft.AspNetCore.Mvc.HttpPostAttribute;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using MyOnlineShop.Models;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;

namespace MyOnlineShop.Controllers
{
    public class SellerController : ControllerBase
    {


        private MyShopContex _context;
        public SellerController(MyShopContex context)
        {
            _context = context;

        }
        //[HttpPost]
        //[Route("sellers/")]
        //public IActionResult GetAllSellers() {
        //    try
        //    {
        //        var seller = _context.sellers.ToList();
        //        if (seller == null)
        //        {
        //            return NotFound();
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        return Ok(seller);
        //    }
        //    catch
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}


        [HttpGet]
        [Route("sellers/")]
        public IActionResult GetAllSellers(int SellersPerPage, int page)
        {   //need to grab list for paging
            try
            {   
                var seller = _context.sellers.ToList();
                List<Models.apimodel.Seller> sellers = new List<Models.apimodel.Seller>();
                foreach(var ss in seller)
                {
                    Models.apimodel.Seller schema = new Models.apimodel.Seller()
                    {
                        information = ss.Information,
                        address = ss.Address,
                        id = ss.ID,
                        dislikes = ss.dislikes,
                        likes = ss.likes,
                        image = ss.user.ImageUrl,
                        name = ss.user.UserName
                    };
                    sellers.Add(schema);
                }

                if (seller == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Models.apimodel.Sellers s = new Models.apimodel.Sellers()
                {
                    sellersPerPage = SellersPerPage,
                    page = page,
                    sellers = sellers

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
                Models.apimodel.Seller schema = new Models.apimodel.Seller()
                {
                    information = ss.Information,
                    address = ss.Address,
                    id = ss.ID,
                    dislikes = ss.dislikes,
                    likes = ss.likes,
                    image = ss.user.ImageUrl,
                    name = ss.user.UserName
                };
                if (ss == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
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
        public ActionResult UpdateSeller(Guid sellerId, [FromBody] Models.apimodel.SellerpagePutMethodRequest s)
        {
            try
            {
                var ss = _context.sellers.SingleOrDefault((p) => p.ID == sellerId);
                Models.apimodel.Seller schema = new Models.apimodel.Seller()
                {
                    information = s.information,
                    address = s.address,
                    id = ss.ID,
                    dislikes = ss.dislikes,
                    likes = ss.likes,
                    image = ss.user.ImageUrl,
                    name = ss.user.UserName
                };
                if (ss == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(schema);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }


    }
}
