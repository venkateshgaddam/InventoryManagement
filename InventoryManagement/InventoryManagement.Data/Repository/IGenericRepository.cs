using InventoryManagement.Data.Entity;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repository
{
    public interface IProductRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        Task<Product> AddProduct(TEntity Product);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        Task<Product> UpdateProduct(TEntity Product);

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
        Task DeleteProduct(TEntity Product);

    }
}
