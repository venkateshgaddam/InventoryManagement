using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagement.Model.EntityModels
{
    public class ErrorResponse
    {
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
    }
}
