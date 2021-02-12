using InventoryManagement.Model.EntityModels;
using Swashbuckle.AspNetCore.Filters;

namespace InventoryManagement.Model.SwaggerExamples
{
    public class AddProductExample : IExamplesProvider<AddProduct>
    {
        public AddProduct GetExamples()
        {
            return new AddProduct()
            {
                Brand = "Nike",
                Category = "Sandals",
                SubCategory = "Sub-Category",
                IsAvailable = true,
                IsDelete = false,
                Price = 4500,
                ProductDescription = "Nike Men's Running Shoes",
                ProductName = "Running Shoes"
            };
        }
    }
}
