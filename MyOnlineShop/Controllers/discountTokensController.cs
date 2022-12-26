using System;
using Microsoft.AspNetCore.Mvc;
using MyOnlineShop.Models.apimodel;

namespace MyOnlineShop.Controllers
{
    public class discountTokensController : ControllerBase
    {
        [HttpGet]
        [Route("discountTokens/{id}/Validate")]
        public ActionResult discountTokenGet(Guid id)
        {
            try
            {
                var s = new Dictionary<string, string>() { { "status", "Valid" } };


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
        [Route("discountTokens/{id}/use")]
        public ActionResult discountTokenPut(Guid id, Guid cartId)
        {
            try
            {
                token s = new token();


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

