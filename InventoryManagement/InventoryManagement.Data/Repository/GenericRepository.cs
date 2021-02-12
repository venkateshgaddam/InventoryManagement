using InventoryManagement.Data.Context;
using InventoryManagement.Data.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Data.Repository
{
    class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        #region Properties

        private readonly DbContextOptions<ProductContext> dbContextOptions;


        #endregion

        #region Constructor

        public GenericRepository()
        {
            dbContextOptions = new DbContextOptionsBuilder<ProductContext>()
                                            .UseInMemoryDatabase(databaseName: "Product")
                                            .Options;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        public async Task<Product> AddProduct(T Product)
        {
            Product ProductDB = Product as Product;
            using (var context = new ProductContext(dbContextOptions))
            {
                //logger.LogInformation($"Db Connvetion Successful");
                context.Products.Add(ProductDB);
                //logger.LogInformation($"Entity Data was added into the Context");

                await context.SaveChangesAsync(acceptAllChangesOnSuccess: true);
                //logger.LogInformation($"Entity Data was saved in the DB");

            }
            return ProductDB;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        public Task DeleteProduct(T Product)
        {
            Product ProductDB = Product as Product;
            using (var context = new ProductContext(dbContextOptions))
            {
                context.Products.Remove(ProductDB);
                context.SaveChanges();
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        public async Task<Product> GetProduct(Guid ProductId)
        {
            using var context = new ProductContext(dbContextOptions);
            return await context.Products.FirstOrDefaultAsync(a => a.ProductId == ProductId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Product"></param>
        /// <returns></returns>
        public async Task<Product> UpdateProduct(T Product)
        {
            Product ProductDB = Product as Product;
            using (var context = new ProductContext(dbContextOptions))
            {
                context.Products.Update(ProductDB);
                await context.SaveChangesAsync();
            }
            return ProductDB;
        }

        #endregion

    }
}
