using System.Collections.Generic;

namespace Im.Common.Database.Interface
{
    public interface IFilterGroup : IFilter
    {
        GroupConditionOperator Operator { get; set; }
        IList<IFilter> Predicates { get; set; }
    }
}