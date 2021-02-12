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
    /// <summary>
    ///     
    /// </summary>
    [Authorize(Roles = UserRoles.Developer)]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductBiz productBiz;
        private  string username;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productBiz"></param>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid ProductId)
        {
            var productData = await productBiz.GetProduct(ProductId);

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
        /// <summary>
        ///      Adds the new Product in the Product InMemory DB 
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        [HttpPost]
        [SwaggerRequestExample(typeof(AddProduct), typeof(AddProductExample))]
        public async Task<IActionResult> Post([FromBody] AddProduct addProduct)
        {
            this.username = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "username")?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                var Product = await this.productBiz.AddProduct(addProduct, username);
                var result = new JsonResult(Product)
                {
                    StatusCode = (int)HttpStatusCode.Created
                };
                return result;
            }
            return Unauthorized();

        }

        /// <summary>
        ///     Updates the Product in the DB
        /// </summary>
        /// <param name="updateProduct"></param>
        /// <returns></returns>
        [HttpPut]
        [SwaggerRequestExample(typeof(UpdateProduct), typeof(UpdateProductExample))]
        public async Task<ActionResult> Put([FromBody] UpdateProduct updateProduct)
        {
            this.username = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "username")?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                var Product = await productBiz.UpdateProduct(updateProduct, username);
                var result = new JsonResult(Product)
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
                return result;
            }
            return Unauthorized();
        }

        /// <summary>
        ///     Implements Soft Delete on the Product Entity therefore the record will bew present in the DB
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [HttpDelete("{productId}")]
        public async Task<IActionResult> Delete(Guid productId)
        {
            username = HttpContext.User.Claims.FirstOrDefault(a => a.Type == "username")?.Value;
            if (!string.IsNullOrEmpty(username))
            {
                await productBiz.DeleteProduct(productId, username);
                return NoContent();
            }
            return Unauthorized();
        }
    }
}
