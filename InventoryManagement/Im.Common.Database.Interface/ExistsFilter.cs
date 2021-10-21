namespace Im.Common.Database.Interface
{
    public class ExistsFilter<T> : IExistsFilter<T>
    {
        public bool Not { get; set; }
        public IFilter filter { get; set; }
        public int Type { get; set; }
    }
}