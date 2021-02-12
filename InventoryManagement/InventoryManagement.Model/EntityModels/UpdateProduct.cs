using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Model.EntityModels
{
    public class UpdateProduct : AddProduct
    {
        public Guid ProductId { get; set; }
    }
}

