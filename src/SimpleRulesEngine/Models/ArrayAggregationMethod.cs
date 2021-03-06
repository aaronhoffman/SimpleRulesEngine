﻿namespace SimpleRulesEngine.Models
{
    public enum ArrayAggregationMethod
    {
        NotSet = 0,
        Sum = 1,
        Min = 2,
        Max = 3,
        Avg = 4,
    }

    public static class ArrayAggregationMethodExtensions
    {
        public static bool HasValue(this ArrayAggregationMethod arrayAggregationMethod)
        {
            return arrayAggregationMethod != ArrayAggregationMethod.NotSet;
        }
    }
}
