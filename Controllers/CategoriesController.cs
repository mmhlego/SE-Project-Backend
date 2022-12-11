using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;




namespace MyOnlineShop.Controllers
{
    public class CategoriesController : ControllerBase
    {
        Categories[] categories = new Categories[999];

        public IEnumerable<Categories> GetAllCategories()
        {
            // Get: Categories
            return categories;
        }
        public ActionResult GetCategory(Guid id)
        {
            var category = categories.FirstOrDefault((p) => p.ID == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }
    }
}
