using InventoryManagement.Model.EntityModels;
using Swashbuckle.AspNetCore.Filters;
using System;

namespace InventoryManagement.Model.SwaggerExamples
{
    public class UpdateProductExample : IExamplesProvider<UpdateProduct>
    {
        public UpdateProduct GetExamples()
        {
            return new UpdateProduct()
            {
                ProductId = Guid.Empty,
                Brand = "Nike",
                Category = "Apparel",
                SubCategory = "Running Shoes",
                IsAvailable = true,
                IsDelete = false,
                Price = 4500,
                ProductDescription = "Nike Men's Running Shoes",
                ProductName = "Running Shoes"
            };
        }
    }
}
