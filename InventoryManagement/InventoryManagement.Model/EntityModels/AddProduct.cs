namespace InventoryManagement.Model.EntityModels
{
    public class AddProduct
    {
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public double Price { get; set; }

        public string Brand { get; set; }

        public string Category { get; set; }

        public string SubCategory { get; set; }

        public bool IsAvailable { get; set; }
        
        public bool IsDelete { get; set; }

    }
}
