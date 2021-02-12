using InventoryManagement.Data.Entity;
using InventoryManagement.Model.EntityModels;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Biz
{
    public interface IProductBiz
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        Task<Product> AddProduct(AddProduct addProduct, string username);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addProduct"></param>
        /// <returns></returns>
        Task<Product> UpdateProduct(UpdateProduct addProduct, string username);

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
        Task DeleteProduct(Guid ProductId, string username);

    }
}
