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
using MyOnlineShop.Models.apimodel;
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
                List<SellerSchema> sellers = new List<SellerSchema>();
                foreach(var ss in seller)
                {
                    User user = _context.users.SingleOrDefault(u => u.ID == ss.UserId);
                    SellerSchema schema = new SellerSchema()
                    {
                        information = ss.Information,
                        address = ss.Address,
                        id = ss.ID,
                        dislikes = ss.dislikes,
                        likes = ss.likes,
                        image = user.ImageUrl,
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
                SellersSchema s = new SellersSchema()
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
                User user = _context.users.SingleOrDefault(u => u.ID == ss.UserId);
                SellerSchema schema = new SellerSchema()
                {
                    information = ss.Information,
                    address = ss.Address,
                    id = ss.ID,
                    dislikes = ss.dislikes,
                    likes = ss.likes,
                    image = user.ImageUrl,
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
        public ActionResult UpdateSeller(Guid sellerId, [FromBody] SellerpagePutMethodRequest s)
        {
            try
            {
                var ss = _context.sellers.SingleOrDefault((p) => p.ID == sellerId);
                User user = _context.users.SingleOrDefault(u => u.ID == ss.UserId);
                SellerSchema schema = new SellerSchema()
                {
                    information = s.information,
                    address = s.address,
                    id = ss.ID,
                    dislikes = ss.dislikes,
                    likes = ss.likes,
                    image = user.ImageUrl,
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
        [Route("sellers/{id:Guid}/likes")]
        public ActionResult putSellerlike(Guid id, [FromBody] likeModel l)
        {
            try
            {
                var seller = _context.sellers.Where(p => p.ID == id).Single();
                if (seller == null)
                    return NotFound();
                else
                {

                    if (l.like == true)
                    {
                        seller.likes = seller.likes + 1;

                    }
                    else
                    {
                        seller.dislikes = seller.dislikes + 1;

                    }
                    _context.Update(seller);
                    _context.SaveChanges();
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    var user = _context.users.SingleOrDefault(s => s.ID == seller.UserId);
                    SellerSchema schema = new SellerSchema()
                    {
                        information = seller.Information,
                        address = seller.Address,
                        id = seller.ID,
                        dislikes = seller.dislikes,
                        likes = seller.likes,
                        image = user.ImageUrl,
                        name = user.FirstName + " " + user.LastName
                        
                    };

                    return Ok(schema);

                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

    }
}
