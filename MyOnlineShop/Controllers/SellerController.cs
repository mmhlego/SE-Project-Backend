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
using System.Xml.Linq;

namespace MyOnlineShop.Controllers
{
    public class SellerController : ControllerBase
    {


        private MyShopContex _context;
        public SellerController(MyShopContex context)
        {
            _context = context;

        }

        [HttpGet]
        [Route("sellers/")]
        public IActionResult GetAllSellers(int SellersPerPage, int page)
        {   
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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
                        return BadRequest();
                    }
                }


                else
                {
                    return NotFound();

                }
                List<SellerSchema> sellerSchema = new List<SellerSchema>(); 
                
                foreach(var ss in seller)
                {
                    var user = _context.users.SingleOrDefault(p => p.ID == ss.UserId);
                    SellerSchema schema = new SellerSchema()
                    {
                        information = ss.Information,
                        address = ss.Address,
                        id = ss.ID,
                        dislikes = ss.dislikes,
                        likes = ss.likes,
                        image = ss.image,
                        name = user.FirstName+" "+user.LastName,
                        restricted = user.Restricted
                    };
                    sellerSchema.Add(schema);
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
                    image = ss.image,
                    name = user.UserName,
                    restricted = user.Restricted
                    
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
                SellerSchema schema = new SellerSchema()
                {
                    information = s.information,
                    address = s.address,
                    id = ss.ID,
                    dislikes = ss.dislikes,
                    likes = ss.likes,
                    image = ss.image,
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
