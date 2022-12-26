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
using MyOnlineShop.Models.apimodel;
using HttpPutAttribute = Microsoft.AspNetCore.Mvc.HttpPutAttribute;
using System.Xml.Linq;

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
        public ActionResult GetAllProducts(ProductPageGetRequestModel p1)

        {

             List<ProductPrice> product1= new List<ProductPrice>();
            if (p1.available == true)
            {
                product1 = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount > 0 && p.product.Category == p1.catagory).ToList();
            }
            else
            {
                product1 = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount == 0 && p.product.Category == p1.catagory).ToList();
            }

             List<ProductPrice> products = new List<ProductPrice>();
       
            if ((p1.page * p1.productsPerPage) - p1.productsPerPage < product1.Count)
            {   
                if (p1.page * p1.productsPerPage > product1.Count)
                {
                    products = product1.GetRange((p1.page * p1.productsPerPage) - p1.productsPerPage, product1.Count);
                
                }
                else
                {
                   
                    products = product1.GetRange((p1.page * p1.productsPerPage) - p1.productsPerPage, p1.productsPerPage);
                }
            }

            List<productModel> productModels = new List<productModel>();
            foreach (ProductPrice productPrice in products)
            {
                productModel p = new () {
                id = productPrice.product.ID,
                image=productPrice.product.Image,
                name=productPrice.product.Name,
                category=productPrice.product.Category,
                description=productPrice.product.Descriptiopn,
                dislikes=1,
                likes=1};
                productModels.Add(p);
            }

            productsModel m = new productsModel()
            {
                page = p1.page,
                productsPerPage = p1.productsPerPage,
                products = productModels
                
            };
            return Ok(m);

        }


        [HttpPost]
        [Route("products/")]
        public ActionResult AddProduct([FromBody] ProductPagePostRequestModel p1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                productsModel product = new productsModel();

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
                var p1 = new productModel()
                {
                    category = products.Category,
                    description = products.Descriptiopn,
                    id = products.ID,
                    dislikes = products.dislikes,
                    likes = products.likes,
                    image = products.Image,
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
                var p1 = new productModel()
                {
                    category = products.Category,
                    description = products.Descriptiopn,
                    id = products.ID,
                    dislikes = products.dislikes,
                    likes = products.likes,
                    image = products.Image,
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
        public ActionResult putProduct(Guid id,[FromBody] ProductPagePostRequestModel p1)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                productsModel product = new productsModel();

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
