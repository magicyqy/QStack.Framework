using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace QStack.Framework.Core.Model
{
    public class Query
    {
        public enum Operators
        {
            None = 0,
            Equal = 1,
            GreaterThan = 2,
            GreaterThanOrEqual = 3,
            LessThan = 4,
            LessThanOrEqual = 5,
            Contains = 6,
            StartWith = 7,
            EndWidth = 8,
            Range = 9,
            In = 10
        }
        public enum Condition
        {
            OrElse = 1,
            AndAlso = 2
        }
        public string Name { get; set; }
        public Operators Operator { get; set; }
        public object Value { get; set; }
        public object ValueMin { get; set; }
        public object ValueMax { get; set; }
    }

    public class ColumnOption
    {

        public string Name { get; set; }
        public bool SearchAble { get; set; }
        public bool OrderAble { get; set; }
        public SearchOption Search { get; set; } = new SearchOption();
    }
    public class SearchOption
    {
        public string Value { get; set; }
        public string ValueMin { get; set; }
        public string ValueMax { get; set; }
        public bool Regex { get; set; }
        public Query.Operators Opeartor { get; set; }
    }
    public class OrderOption
    {
        public string Column { get; set; }
        //asc|desc
        public string Dir { get; set; }

        public bool IaValidDir()
        {
            if (string.IsNullOrWhiteSpace(Dir))
                return false;

            return Regex.IsMatch(Dir.Trim(), @"^(asc|desc)$", RegexOptions.IgnoreCase);
        }
    }
}
