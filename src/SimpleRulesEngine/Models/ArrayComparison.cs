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
        /// This option is valid when the left argument is an array and the right argument is an array.
        /// </summary>
        ContainsAny = 3,

        /// <summary>
        /// This option is valid when the left argument is an array and the right argument is an array.
        /// </summary>
        DoesNotContainAny = 4,

        /// <summary>
        /// This option is valid when the left argument is an array and the right argument is an array.
        /// </summary>
        ContainsAll = 5,

        // todo: opportunity to consolidate with PerformArraysComparison
        // e.g `IsAny` can be translated to `ContainsAny`
    }

    public static class ArrayComparisonExtensions
    {
        public static bool IsValidArraysComparison(this ArrayComparison? arrayComparison)
        {
            return
                arrayComparison.HasValue
                && (arrayComparison.Value == ArrayComparison.ContainsAll
                    || arrayComparison.Value == ArrayComparison.ContainsAny
                    || arrayComparison.Value == ArrayComparison.DoesNotContainAny);
        }
    }
}
