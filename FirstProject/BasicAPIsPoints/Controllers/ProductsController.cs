using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BasicAPIsPoints.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ProductsController : ControllerBase
    {
        [HttpGet]

        public ActionResult<List<string>> Get()
        {
            return new List<string> { "Apple", "Banana", "Orange" };
        }
    }
}