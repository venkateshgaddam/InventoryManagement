using System.Collections.Generic;

namespace Carrier.CCP.Common.Database.Interface
{
    public interface IFilterGroup : IFilter
    {
        GroupConditionOperator Operator { get; set; }
        IList<IFilter> Predicates { get; set; }
    }
}