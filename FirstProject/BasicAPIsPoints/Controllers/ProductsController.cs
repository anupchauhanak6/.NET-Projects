using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BasicAPIsPoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController : ControllerBase
    {
        public List<string> fruits = new List<string>{ "Apple", "Banana", "Orange" };
        // 1. old method: for all products (api/products)
        [HttpGet]
        public ActionResult<List<string>> Get()
        {
            return Ok(fruits);
        }

        // new method: for any one product (api/products/1)
        [HttpGet("{id}")]
        public ActionResult<string> GetById(int id)
        {

            int index = id - 1;

            if (index < 0 || index >= fruits.Count)
            {
                return NotFound($"Product with ID {id} nahi mila!");
            }
            return Ok(fruits[index]);
        }
    }
}
