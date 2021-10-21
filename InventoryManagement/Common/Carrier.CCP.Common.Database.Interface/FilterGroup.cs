using System.Collections.Generic;

namespace Carrier.CCP.Common.Database.Interface
{
    public class FilterGroup : IFilterGroup
    {
        public GroupConditionOperator Operator { get; set; }
        public IList<IFilter> Predicates { get; set; }
        public int Type { get; set; }
    }
}