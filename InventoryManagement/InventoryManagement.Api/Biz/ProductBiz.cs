using AutoMapper;
using InventoryManagement.Api.Service;
using InventoryManagement.Data.Entity;
using InventoryManagement.Data.Repository;
using InventoryManagement.Model.EntityModels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace InventoryManagement.Api.Biz
{

    public class ProductBiz : IProductBiz
    {
        #region Properties

        private readonly IMapper mapper;

        private readonly IProductService productService;
        private readonly ILogger<Product> _Logger;

        #endregion

        #region Constructor

        public ProductBiz(IProductService productService, ILogger<Product> _Logger)
        {
            this.productService = productService;
            this._Logger = _Logger;
        }

        #endregion

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

        public Task DeleteProduct(Guid ProductId, string username)
        {
            throw new NotImplementedException();
        }

        public Task<Product> GetProduct(Guid ProductId)
        {
            throw new NotImplementedException();
        }

        public async Task<Product> UpdateProduct(UpdateProduct addProduct, string username)
        {
            try
            {
                _Logger.LogInformation("Validation Success");
                Product Product = mapper.Map<Product>(addProduct);
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
