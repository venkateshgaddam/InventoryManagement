using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Invoicing
{
    public abstract class Order
    {
        public List<Product> products { get; set; }


        public Order(List<Product> products)
        {
            this.products = products;
        }


        public int GetProductsCount() { return this.products.Count; }

        public Product GetProductDetails(string productName) { return products.FirstOrDefault(a => a.product.Contains(productName.TrimEnd())); }

        public abstract void PrintInvoice(string[] input);
    }
}
