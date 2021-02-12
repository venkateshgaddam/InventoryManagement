using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Data.Entity
{
    public class Product
    {
        public Guid ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public double Price { get; set; }

        public string Brand { get; set; }

        public int Category { get; set; }

        public int SubCategory { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsDelete { get; set; }

        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

    }
}
