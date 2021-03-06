﻿namespace SimpleRulesEngine.Models
{
    public enum NumericComparison
    {
        NotSet = 0,
        Equal = 1,
        NotEqual = 2,
        GreaterThan = 3,
        GreaterThanOrEqual = 4,
        LessThan = 5,
        LessThanOrEqual = 6,
    }

    public static class NumericComparisonExtensions
    {
        public static bool HasValue(this NumericComparison numericComparison)
        {
            return numericComparison != NumericComparison.NotSet;
        }
    }
}
