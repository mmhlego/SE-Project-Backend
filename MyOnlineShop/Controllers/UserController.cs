using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using HttpGetAttribute = Microsoft.AspNetCore.Mvc.HttpGetAttribute;
using MyOnlineShop.Data;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace MyOnlineShop.Controllers
{
    public class UserController : ControllerBase
    {
        private MyShopContex _context;
        public UserController(MyShopContex context)
        {
            _context = context;
            
        }


        [HttpGet]
        [Route("user/{id}")]        
        public ActionResult GetUser(Guid id)
        {
            var user = _context.users.SingleOrDefault(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
    }
}
