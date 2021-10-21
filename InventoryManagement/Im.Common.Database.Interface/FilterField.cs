using System;
using System.Linq.Expressions;

namespace Im.Common.Database.Interface
{
    public class FilterField<T> : IFilterField<T>
    {
        public bool Not { get; set; }
        public string FieldName { get; set; }
        public ConditionOperator Operator { get; set; }
        public Expression<Func<T, object>> Predicate { get; set; }
        public int Type { get; set; }
        public object Value { get; set; }
        public bool IgnoreCase { get; set; } = true;
    }
}