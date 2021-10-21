namespace Im.Common.Database.Interface
{
    public class Order : IOrder
    {
        public string PropertyName { get; set; }
        public bool Ascending { get; set; }
    }
}