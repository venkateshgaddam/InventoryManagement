using InventoryManagement.Api.Biz;
using InventoryManagement.Model.EntityModels;
using InventoryManagement.Model.SwaggerExamples;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
namespace InventoryManagement.Api.Controllers
{
    [Authorize(Roles = UserRoles.Developer)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductBiz productBiz;

        public ProductController(IProductBiz productBiz)
        {
            this.productBiz = productBiz;
        }


        // GET: api/<ProductController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProductController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid ProductId)
        {
            var productData = await this.productBiz.GetProduct(ProductId);

            if (productData != null)
            {
                return new JsonResult(productData)
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return NotFound();
        }

        // POST api/<ProductController>
        [HttpPost]
        [SwaggerRequestExample(typeof(AddProduct), typeof(AddProductExample))]
        public async Task<IActionResult> Post([FromBody] AddProduct addProduct)
        {
            string username = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "")?.Value;

            var Product = await this.productBiz.AddProduct(addProduct, username);
            var result = new JsonResult(Product)
            {
                StatusCode = (int)HttpStatusCode.Created
            };
            return result;
        }

        // PUT api/<ProductController>/5
        [HttpPut]
        [SwaggerRequestExample(typeof(UpdateProduct), typeof(UpdateProductExample))]
        public async Task<ActionResult> Put([FromBody] UpdateProduct updateProduct)
        {
            string username = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "")?.Value;

            var Product = await productBiz.UpdateProduct(updateProduct, username);
            var result = new JsonResult(Product)
            {
                StatusCode = (int)HttpStatusCode.OK
            };
            return result;
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{productId}")]
        public async Task Delete(Guid productId)
        {
            string username = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "")?.Value;
            await productBiz.DeleteProduct(productId, username);
        }
    }
}
