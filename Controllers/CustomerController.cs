using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using MyOnlineShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Web.Mvc;

namespace MyOnlineShop.Controllers
{
    public class CustomerController : ControllerBase
    {
        Customer[] customers = new Customer[999];

        public IEnumerable<Customer> GetAllCustomers()
        {
            // Get: Customers
            return customers;
        }
        public ActionResult GetCustomer(int id)
        {
            var customer = customers.FirstOrDefault((p) => p.ID == id);
            if (customer== null)
            {
                return NotFound();
            }
            return Ok(customer);
        }
    }
}
