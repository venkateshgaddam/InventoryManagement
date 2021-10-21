namespace Im.Common.Database.Interface
{
    public interface IExistsFilter<T> : IFilter
    {
        public bool Not { get; set; }
        public IFilter filter { get; set; }
    }
}