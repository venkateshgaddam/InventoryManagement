namespace Carrier.CCP.Common.Database.Interface
{
    public interface IOrder
    {
        string PropertyName { get; set; }
        bool Ascending { get; set; }
    }
}