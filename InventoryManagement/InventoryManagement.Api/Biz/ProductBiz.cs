using AutoMapper;
using InventoryManagement.Api.Service;
using InventoryManagement.Data.Entity;
using InventoryManagement.Model.EntityModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Biz
{
    /// <summary>
    /// 
    /// </summary>
    public class ProductBiz : IProductBiz
    {
        #region Properties

        private readonly IMapper mapper;

        private readonly IProductService productService;
        private readonly ILogger<Product> _Logger;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productService"></param>
        /// <param name="_Logger"></param>
        public ProductBiz(IProductService productService, ILogger<Product> _Logger)
        {
            this.productService = productService;
            this._Logger = _Logger;
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AddProduct, Product>();
                cfg.CreateMap<UpdateProduct, Product>();
            });
            mapper = config.CreateMapper();
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addProduct"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<Product> AddProduct(AddProduct addProduct, string username)
        {
            try
            {
                _Logger.LogInformation("Validation Success");
                Product Product = mapper.Map<Product>(addProduct);
                Product.CreatedDate = DateTimeOffset.Now;
                Product.CreatedBy = username;

                _Logger.LogDebug($"The Object which gets inserted into the DB :- {JsonConvert.SerializeObject(Product) }");
                Product ProductfromDB = await productService.AddProduct(Product);
                return ProductfromDB;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error Occured while processing the Add Request. {ex.Message}");
                throw;
            }
        }

        /// <summary>
        ///     Soft Delete 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task DeleteProduct(Guid ProductId, string username)
        {
            try
            {
                _Logger.LogInformation("Validation Success");
                Product Product = await productService.GetProduct(ProductId);
                Product.IsAvailable = false;
                Product.IsDelete = true;
                Product.UpdatedDate = DateTimeOffset.Now;
                Product.UpdatedBy = username;

                _Logger.LogDebug($"The Object which gets Updated into the DB :- {JsonConvert.SerializeObject(Product) }");
                Product ProductfromDB = await productService.UpdateProduct(Product);
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error Occured while processing the Add Request. {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<Product> GetProduct(Guid ProductId)
        {
            Product Product = await productService.GetProduct(ProductId);
            return Product;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateProduct"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<Product> UpdateProduct(UpdateProduct updateProduct, string username)
        {
            try
            {

                _Logger.LogInformation("Validation Success");
                Product Product = await productService.GetProduct(updateProduct.ProductId);
                if (Product == null)
                {
                    throw new Exception($"No Product with Id {updateProduct.ProductId} exists in the DB");
                }
                Product.UpdatedDate = DateTimeOffset.Now;
                Product.UpdatedBy = username;

                _Logger.LogDebug($"The Object which gets Updated into the DB :- {JsonConvert.SerializeObject(Product) }");
                Product ProductfromDB = await productService.UpdateProduct(Product);
                return ProductfromDB;
            }
            catch (Exception ex)
            {
                _Logger.LogError($"Error Occured while processing the Add Request. {ex.Message}");
                throw;
            }
        }
    }
}
