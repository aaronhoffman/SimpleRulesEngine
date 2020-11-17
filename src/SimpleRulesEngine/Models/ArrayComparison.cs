namespace SimpleRulesEngine.Models
{
    public enum ArrayComparison
    {
        NotSet = 0,

        /// <summary>
        /// This option is valid when the left argument is a single value and the right argument is an array.
        /// </summary>
        IsAny = 1,

        /// <summary>
        /// This option is valid when the left argument is a single value and the right argument is an array.
        /// </summary>
        IsNotAny = 2,


        /// <summary>
        /// This option is valid when both the left argument and the right argument are arrays.
        /// </summary>
        ContainsAny = 3,

        /// <summary>
        /// This option is valid when both the left argument and the right argument are arrays.
        /// </summary>
        DoesNotContainAny = 4,

        /// <summary>
        /// This option is valid when both the left argument and the right argument are arrays.
        /// </summary>
        ContainsAll = 5,

        // todo: opportunity to consolidate.
        // e.g `IsAny` can be translated to `ContainsAny`
    }

    public static class ArrayComparisonExtensions
    {
        public static bool HasValue(this ArrayComparison arrayComparison)
        {
            return arrayComparison != ArrayComparison.NotSet;
        }

        /// <summary>
        /// Value is a valid Left Array <> Right Array comparison value.
        /// </summary>
        /// <param name="arrayComparison"></param>
        /// <returns></returns>
        public static bool IsValidArraysComparison(this ArrayComparison arrayComparison)
        {
            return arrayComparison == ArrayComparison.ContainsAll
                || arrayComparison == ArrayComparison.ContainsAny
                || arrayComparison == ArrayComparison.DoesNotContainAny;
        }
    }
}
