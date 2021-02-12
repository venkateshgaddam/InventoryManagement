using InventoryManagement.Data.Entity;
using InventoryManagement.Data.Repository;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Service
{
    public class ProductService : IProductService
    {
        readonly IGenericRepository<Product> productRepository;

        public ProductService(IGenericRepository<Product> genericRepository)
        {
            this.productRepository = genericRepository;
        }
        public async Task<Product> AddProduct(Product addProduct)
        {
            var result = await productRepository.AddProduct(addProduct);
            return result;
        }

        public async Task DeleteProduct(Product Product)
        {
            await this.productRepository.DeleteProduct(Product);
        }

        public async Task<Product> GetProduct(Guid ProductId)
        {
            return await productRepository.GetProduct(ProductId);
        }

        public async Task<Product> UpdateProduct(Product addProduct)
        {
            var result = await productRepository.UpdateProduct(addProduct);
            return result;
        }
    }
}
