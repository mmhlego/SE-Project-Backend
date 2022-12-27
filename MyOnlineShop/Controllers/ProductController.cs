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

            List<ProductPrice> product1 = _context.productPrices.ToList();
          
             if (p1.available == true)
            {
                product1 = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount > 0).ToList();
            }
            else
            {
                product1 = _context.productPrices.Where(p => p.Price >= p1.priceFrom && p.Price <= p1.priceTo && p.Amount == 0).ToList();
            }
             
             if(p1.catagory != null)
            {
                List<ProductPrice> productPrices = new List<ProductPrice>();
                foreach(ProductPrice p in product1)
                {
                    var item = _context.Products.SingleOrDefault(x => x.ID == p.ProductID);
                    if (item.Category == p1.catagory)
                    {
                        productPrices.Add(p);
                    }
                }
                product1 = productPrices;
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
                var eachproduct = _context.Products.SingleOrDefault(p => p.ID == productPrice.ProductID);
                productModel p = new productModel() {
                id = eachproduct.ID,
                image= eachproduct.Image,
                name= eachproduct.Name,
                category= eachproduct.Category,
                description= eachproduct.Descriptiopn,
                dislikes= eachproduct.dislikes,
                likes= eachproduct.likes};
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





        [HttpPut]
        [Route("products/{id:Guid}/likes")]
        public ActionResult putProductlike(Guid id, [FromBody] likeModel l)
        {
            try
            {
                var product = _context.Products.Where(p => p.ID == id).Single();
                if (product == null)
                    return NotFound();
                else
                {

                    if (l.like == true)
                    {
                        product.likes = product.likes + 1;
                        
                    }
                    else
                    {
                        product.dislikes = product.dislikes + 1;
                        
                    }
                    _context.Update(product);
                    _context.SaveChanges();
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }

                    productModel products = new productModel()
                    {
                        category = product.Category,
                        description = product.Descriptiopn,
                        id = product.ID,
                        dislikes = product.dislikes,
                        likes = product.likes,
                        image = product.Image,
                        name = product.Name


                    };

                    return Ok(products);

                }
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

       
    }

}
