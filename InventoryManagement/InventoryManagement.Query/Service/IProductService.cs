using InventoryManagement.Data.Entity;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Service
{
    public interface IProductService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        Task<Product> AddProduct(Product addProduct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        Task<Product> UpdateProduct(Product addProduct);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<Product> GetProduct(Guid ProductId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task DeleteProduct(Product ProductId);
    }
}
