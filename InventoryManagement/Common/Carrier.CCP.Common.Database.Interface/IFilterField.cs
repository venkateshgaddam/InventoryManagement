using System;
using System.Linq.Expressions;

namespace Carrier.CCP.Common.Database.Interface
{
    public interface IFilterField<T> : IFilter
    {
        ConditionOperator Operator { get; set; }
        Expression<Func<T, object>> Predicate { get; set; }
        object Value { get; set; }
    }
}