using System;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models;
using MyOnlineShop.Models.apimodel;

namespace MyOnlineShop.Controllers
{
    public class AdminController : ControllerBase
    {
        [HttpGet]
        [Route("admin/users")]
        public ActionResult userssget(int customerPerPage, int Page)
        {
            try
            {
                usersModel s = new usersModel();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }
        [HttpGet]
        [Route("admin/users/{id}")]
        public ActionResult eachuserget(Guid id)
        {
            try
            {
                userModel s = new userModel();


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
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
                userModel s = new userModel();


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
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
                userModel s = new userModel();


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }



        [HttpGet]
        [Route("admin/discountTokens")]
        public ActionResult discountTokens(bool isEvent, bool expired)
        {
            try
            {
                tokensModel s = new tokensModel();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }



        [HttpPost]
        [Route("admin/discountTokens")]
        public ActionResult discountTokenspost([FromBody] tokenreqModel tokenreq)
        {
            try
            {
                DiscountToken s = new DiscountToken();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }




        [HttpDelete]
        [Route("admin/discountTokens/{id}")]
        public ActionResult discountTokensdelete( Guid id)
        {
            try
            {
                DiscountToken s = new DiscountToken();
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }

        [HttpGet]
        [Route("admin/carts")]
        public ActionResult admincarts(Guid userId , bool current)
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
        [Route("admin/carts/{id:Guid}")]
        public ActionResult admincart(Guid id)
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


      

        [HttpPut]
        [Route("admin/carts/{id}")]
        public ActionResult admincartsput(Guid id)
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



        [HttpGet]
        [Route("admin/stats")]
        public ActionResult sellerstate(Guid productId, Guid sellerId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                statsModel s = new statsModel();


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(s);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }



        }


    }
}

    

