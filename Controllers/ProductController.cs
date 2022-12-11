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
using FromBodyAttribute = Microsoft.AspNetCore.Mvc.FromBodyAttribute;
using HttpDeleteAttribute = Microsoft.AspNetCore.Mvc.HttpDeleteAttribute;

using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;

namespace MyOnlineShop.Controllers
{
    public class ProductController : ControllerBase
    {
        private MyShopContex _context;
        public ProductController(MyShopContex context)
        {
            _context = context;

        }

        [HttpGet]
        [Route("products/")]
        public ActionResult GetAllProducts(Models.apimodel.ProductPageGetRequestModel p1)

        {

            //List<ProductPrice> product= new List<ProductPrice>();
            //if (p1.available == true)
            //{
            //    product = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount > 0).ToList();
            //}
            //else
            //{
            //    product = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount == 0).ToList();
            //}
            //List<Product> products1 = new List<Product>();
            //foreach (var p2 in product)
            //{
            //    products1.Add(_context.Products.SingleOrDefault(p => p.ID == p2.ProductID));
            //}
            //List<Product> products3 = new List<Product>();
            //products3 = products1.Where(p => p.CategoriesID == p1.catagory).ToList();

            //int a = p1.page * p1.productsPerPage;
            //if (products3.Count < p1.productsPerPage)
            //    p1.productsPerPage = products3.Count;
            //var product2 = products3.GetRange(a - p1.productsPerPage, p1.productsPerPage);
            //if (product2 == null)
            //    return NotFound();
            //ProductPageResponseModel m = new ProductPageResponseModel()
            //{
            //    page = p1.page,
            //    productsPerPage = p1.productsPerPage,
            //    products = product2
            //}
            //;
            Models.apimodel.Products m = new Models.apimodel.Products();
            return Ok(m);

        }


        [HttpPost]
        [Route("products/")]
        public ActionResult AddProduct([FromBody] Models.apimodel.ProductPagePostRequestModel p1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Models.apimodel.Product product = new Models.apimodel.Product();

                return Ok(product);

            }

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }


     
        [HttpGet]
        [Route("products/{id:Guid}")]
        public ActionResult GetProduct(Guid id)
        {
            try
            {
                var products = _context.Products.SingleOrDefault((p) => p.ID == id);
                var p1 = new Models.apimodel.Product()
                {
                    category = products.Categories.Name,
                    description = products.Descriptiopn,
                    id = products.ID,
                    dislikes = products.dislikes,
                    likes = products.likes,
                    image = products.Url,
                    name = products.Name

                };
                if (products == null)
                {
                    return NotFound();
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                return Ok(p1);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

       

        [HttpDelete]
        [Route("products/{id:Guid}")]
        public ActionResult DeleteProduct(Guid id)
        {
            try
            {
                var products = _context.Products.SingleOrDefault((p) => p.ID == id);
                var p1 = new Models.apimodel.Product()
                {
                    category = products.Categories.Name,
                    description = products.Descriptiopn,
                    id = products.ID,
                    dislikes = products.dislikes,
                    likes = products.likes,
                    image = products.Url,
                    name = products.Name

                };
                if (products == null)
                {
                    return NotFound();
                }
                else _context.Products.Remove(products);
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                return Ok(p1);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }



        [HttpPut]
        [Route("products/{id:Guid}")]
        public ActionResult putProduct(Guid id,[FromBody] Models.apimodel.ProductPagePostRequestModel p1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Models.apimodel.Products product = new Models.apimodel.Products();

                return Ok(product);

            }

            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        //[HttpGet]
        //[Route("products/{id:Guid}/sellers")]
        //public ActionResult GetSellerProduct(Guid id)
        //{
        //    try
        //    {
        //        var sellers = _context.productPrices.Where(p => p.ProductID == id);
        //        if (sellers == null)
        //        {
        //            return NotFound();
        //        }
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //        return Ok(sellers);
        //    }
        //    catch
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
        //[HttpPost]
        //[Route("products/")]
        //public IActionResult GetAllProducts([FromBody] Product product)
        //{

        //    var products = _context.Products.ToList();
        //    if (products == null)
        //    {
        //        return NotFound();
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    return Ok(products);

        //}

        //[HttpPost]
        //[Route("products/")]
        //public async Task<ActionResult<ProductPageResponseModel > > ShowallProducts([FromBody] ProductPageRequestModel reqbodyjson)
        //{
        //    try {
        //        if (reqbodyjson == null)
        //            return BadRequest();
        //        var products = _context.Products.Where(p => p.);
        //        ProductPageResponseModel res = new ProductPageResponseModel()
        //        { page = reqbodyjson.page,
        //            productsPerPage = reqbodyjson.productsPerPage,




        //        }
        //    }
        //    catch (Exception){

        //        return StatusCode(StatusCodes.Status500InternalServerError);
        //    }
        //}
    }

}
